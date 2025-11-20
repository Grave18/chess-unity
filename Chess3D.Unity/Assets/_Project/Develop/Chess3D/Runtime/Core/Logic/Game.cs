using System.Collections.Generic;
using System.Linq;
using VContainer;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;

namespace Chess3D.Runtime.Core.Logic
{
    public sealed class Game : ILoadUnit
    {
        public PieceColor CurrentTurnColor { get; private set; } = PieceColor.White;
        public PieceColor PreviousTurnColor => CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        public CheckType CheckType { get; private set; } = CheckType.None;
        public string EndGameDescription { get; private set; }
        public HashSet<Square> UnderAttackSquares { get; private set; } = new();
        public AttackLinesList AttackLines { get; } = new();

        private UciBuffer _uciBuffer;
        private Board _board;
        private FenFromString _fenFromString;
        private SettingsService _settingsService;

        /// Is it turn of current player
        public bool IsMyTurn => CurrentTurnColor == _settingsService.S.GameSettings.PlayerColor;
        public bool IsWhiteTurn => CurrentTurnColor == PieceColor.White;
        public bool IsBlackTurn => CurrentTurnColor == PieceColor.Black;

        public HashSet<Piece> CurrentTurnPieces => CurrentTurnColor == PieceColor.White ? _board.WhitePieces : _board.BlackPieces;
        public HashSet<Piece> PrevTurnPieces => CurrentTurnColor == PieceColor.Black ? _board.WhitePieces : _board.BlackPieces;

        // End game
        private PieceColor _winnerColor;

        public bool IsCheck => CheckType is CheckType.Check or CheckType.DoubleCheck;
        public bool IsCheckmate => CheckType is CheckType.Checkmate;
        public bool IsDraw => CheckType == CheckType.Draw;
        public bool IsTimeOut => CheckType is CheckType.TimeOut;
        public bool IsResign => CheckType is CheckType.Resign;
        public bool IsEndGame => IsCheckmate || IsDraw || IsTimeOut || IsResign;
        public bool IsWinnerWhite => _winnerColor == PieceColor.White;
        public bool IsWinnerBlack => _winnerColor == PieceColor.Black;


        [Inject]
        public void Construct(Board board, FenFromString fenFromString, UciBuffer uciBuffer, SettingsService settingsService)
        {
            _board = board;
            _fenFromString = fenFromString;
            _uciBuffer = uciBuffer;
            _settingsService = settingsService;
        }

        public UniTask Load()
        {
            ResetGameState();
            PreformCalculations();

            return UniTask.CompletedTask;
        }


        private void ResetGameState()
        {
            CheckType = CheckType.None;
            CurrentTurnColor = _fenFromString.TurnColor;
            Selected = null;
        }

        public void ChangeTurn()
        {
            CurrentTurnColor = CurrentTurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }

        // TODO: This functionality must be in Flow
        // public void Rematch()
        // {
        //     Competitors.Restart();
        //     Board.Build();
        //     StartGame().Forget();
        // }

        public void TimeOutSetup(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White)
            {
                CheckType = CheckType.TimeOut;
                _winnerColor = PieceColor.Black;
                EndGameDescription = "White is timeout";
            }
            else if (pieceColor == PieceColor.Black)
            {
                CheckType = CheckType.TimeOut;
                _winnerColor = PieceColor.White;
                EndGameDescription = "Black is timeout";
            }
        }

        public void CheckmateSetup()
        {
            CheckType = CheckType.Checkmate;
            _winnerColor = PreviousTurnColor;

            string winnerString = _winnerColor == PieceColor.White ? "White" : "Black";
            EndGameDescription = $"{winnerString} winning the game";
        }

        public void DrawSetup(string description)
        {
            CheckType = CheckType.Draw;
            EndGameDescription = description;
            _winnerColor = PieceColor.None;
        }

        public void ResignSetup()
        {
            CheckType = CheckType.Resign;
            EndGameDescription = $"{CurrentTurnColor} player resigned";
            _winnerColor = PreviousTurnColor;
        }

        /// Calculate under attack squares, check type, moves, captures, draw
        public void PreformCalculations()
        {
            UnderAttackSquares = GetUnderAttackSquares(PrevTurnPieces);
            CheckType = CalculateCheck(PrevTurnPieces);

            foreach (Piece piece in CurrentTurnPieces)
            {
                piece.CalculateMovesAndCaptures();
                piece.CalculateConstrains();
            }

            CalculateEndGame(CurrentTurnPieces);
        }

        private static HashSet<Square> GetUnderAttackSquares(HashSet<Piece> pieces)
        {
            var underAttackSquares = new HashSet<Square>();
            foreach (Piece piece in pieces)
            {
                piece.CalculateMovesAndCaptures();
                FillUnderAttackSquaresForPiece(piece, underAttackSquares);
            }

            return underAttackSquares;
        }

        private static void FillUnderAttackSquaresForPiece(Piece piece, HashSet<Square> underAttackSquares)
        {
            // Pawn's under attack
            if (piece is Pawn pawn)
            {
                var attackSquares = new List<Square>(pawn.UnderAttackSquares);
                foreach (Square underAttackSquare in attackSquares)
                {
                    underAttackSquares.Add(underAttackSquare);
                }
            }
            // Other piece's under attack
            else
            {
                var moveSquares = new List<Square>(piece.MoveSquares.Keys);
                foreach (Square moveSquare in moveSquares)
                {
                    underAttackSquares.Add(moveSquare);
                }
            }

            // All pieces defends
            foreach (Square defendSquare in piece.DefendSquares)
            {
                underAttackSquares.Add(defendSquare);
            }
        }

        private CheckType CalculateCheck(HashSet<Piece> pieces)
        {
            AttackLines.Clear();

            foreach (Piece piece in pieces)
            {
                // Fill under attack line
                bool isCheck = IsPieceMakeCheck(piece);
                if (piece is LongRange longRange)
                {
                    if (!longRange.HasAttackLine) continue;

                    var attackLine = new AttackLine(piece, isCheck, longRange.AttackLineSquares, longRange.SquareBehindKing);
                    AttackLines.Add(attackLine);
                }
                else
                {
                    if (!isCheck) continue;

                    var attackLine = new AttackLine(piece, true);
                    AttackLines.Add(attackLine);
                }
            }

            CheckType check = AttackLines.GetCheckCount() switch
            {
                0 => CheckType.None,
                1 => CheckType.Check,
                _ => CheckType.DoubleCheck
            };

            return check;

            bool IsPieceMakeCheck(Piece piece)
            {
                foreach ((Square square, _) in piece.CaptureSquares)
                {
                    if (square.HasPiece() && square.GetPiece() is King king &&
                        king.GetPieceColor() != piece.GetPieceColor())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void CalculateEndGame(HashSet<Piece> currentTurnPieces)
        {
            if (IsAnyPieceHasMove(currentTurnPieces))
            {
                if (IsThreefoldRule())
                {
                    DrawSetup("By threefold repetition");
                }
                else if (IsRule50())
                {
                    DrawSetup("By fifty move rule");
                }
                else if(IsInsufficientMaterial(out string checkDescription))
                {
                    DrawSetup(checkDescription);
                }
            }
            else
            {
                switch (CheckType)
                {
                    // Stalemate
                    case CheckType.None:
                        DrawSetup("Stalemate. Players have no moves");
                        break;
                    // Checkmate
                    case CheckType.Check or CheckType.DoubleCheck:
                        CheckmateSetup();
                        break;
                }
            }
        }

        private static bool IsAnyPieceHasMove(HashSet<Piece> currentTurnPieces)
        {
            return currentTurnPieces.Any(piece => piece.MoveSquares.Count  > 0
                                                  || piece.CaptureSquares.Count > 0);
        }

        private bool IsThreefoldRule()
        {
            bool isThreefold = _uciBuffer.ThreefoldRepetitionCount >= _settingsService.S.GameSettings.ThreefoldRepetitionCount;

            return isThreefold;
        }

        private bool IsRule50()
        {
            return _uciBuffer.HalfMoveClock == _settingsService.S.GameSettings.FiftyMoveRuleCount * 2;
        }

        private bool IsInsufficientMaterial(out string checkDescription)
        {
            checkDescription = string.Empty;
            bool isInsufficientPieces = IsInsufficientPiecesOneSide(CurrentTurnPieces, PrevTurnPieces, out checkDescription)
                          || IsInsufficientPiecesOneSide(PrevTurnPieces, CurrentTurnPieces, out checkDescription);

            return isInsufficientPieces;
        }

        private static bool IsInsufficientPiecesOneSide(HashSet<Piece> thisSide, HashSet<Piece> otherSide, out string checkDescription)
        {
            checkDescription = string.Empty;
            bool oneKingThisSide = thisSide.Count == 1 && thisSide.First() is King;
            bool isDraw = oneKingThisSide && IsInsufficientFiguresInOtherSide(otherSide, out checkDescription);

            return isDraw;
        }

        /// K-K; K-KN; K-KNN; K-KB
        private static bool IsInsufficientFiguresInOtherSide(HashSet<Piece> otherSide, out string checkDescription)
        {
            checkDescription = string.Empty;
            bool hasKing = otherSide.Count > 0 && otherSide.Any(p => p is King);

            // K vs K
            bool oneKing = otherSide.Count == 1 && hasKing;
            if (oneKing)
            {
                checkDescription = "Insufficient material. Both sides has only one king";
                return true;
            }

            // K vs KB
            bool kingAndBishop = otherSide.Count == 2 && hasKing && otherSide.Any(p => p is Bishop);
            if (kingAndBishop)
            {
                checkDescription = "Insufficient material. One side has one king and other king and bishop";
                return true;
            }

            // K vs KN
            bool kingAndKnight = otherSide.Count == 2 && hasKing && otherSide.Any(p => p is Knight);
            if (kingAndKnight)
            {
                checkDescription = "Insufficient material. One side has one king and other king and knight";
                return true;
            }

            // K vs KNN
            bool kingAnd2Knights = otherSide.Count == 3 && hasKing && otherSide.OfType<Knight>().Count() == 2;
            if (kingAnd2Knights)
            {
                checkDescription = "Insufficient material. One side has one king and other king and 2 knights";
                return true;
            }

            return false;
         }

#region Selection

        public ISelectable Selected { get; private set; }

        public bool CanSelect(ISelectable selectable)
        {
            return selectable.HasPiece() && selectable.GetPieceColor() == CurrentTurnColor;
        }

        public void Select(ISelectable selectable)
        {
            Selected = selectable;
        }

        /// Deselect currently selected piece
        public void Deselect()
        {
            Selected = null;
        }

#endregion
    }
}