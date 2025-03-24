using System.Collections.Generic;
using System.Linq;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.MovesBuffer;

namespace Logic.Players.GameStates
{
    public class IdleState : GameState
    {
        private bool _isRunning;

        public IdleState(Game game) : base(game)
        {
        }

        public override string Name => "Idle";

        public override void Enter()
        {
            CalculateCurrentTurn();
            _isRunning = true;
            Game.Competitors.StartPlayer();
        }

        private void CalculateCurrentTurn()
        {
            Game.UnderAttackSquares = GetUnderAttackSquares(Game.PrevTurnPieces);
            Game.CheckType = CalculateCheck(Game.PrevTurnPieces);

            foreach (Piece piece in Game.CurrentTurnPieces)
            {
                piece.CalculateMovesAndCaptures();
                piece.CalculateConstrains();
            }

            CalculateCheckMateOrStalemate(Game.CurrentTurnPieces);
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
            Game.AttackLines.Clear();

            foreach (Piece piece in pieces)
            {
                // Fill under attack line
                bool isCheck = IsPieceMakeCheck(piece);
                if (piece is LongRange longRange)
                {
                    if (!longRange.HasAttackLine) continue;

                    var attackLine = new AttackLine(piece, isCheck, longRange.AttackLineSquares, longRange.SquareBehindKing);
                    Game.AttackLines.Add(attackLine);
                }
                else
                {
                    if (!isCheck) continue;

                    var attackLine = new AttackLine(piece, true);
                    Game.AttackLines.Add(attackLine);
                }
            }

            return Game.AttackLines.GetCheckCount() switch
            {
                0 => CheckType.None,
                1 => CheckType.Check,
                _ => CheckType.DoubleCheck
            };

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

        private void CalculateCheckMateOrStalemate(HashSet<Piece> currentTurnPieces)
        {
            // If all pieces have no moves
            if (currentTurnPieces.Any(piece => piece.MoveSquares.Count  > 0
                                               || piece.CaptureSquares.Count > 0))
            {
                return;
            }

            Game.CheckType = Game.CheckType switch
            {
                CheckType.None => CheckType.Stalemate,
                CheckType.Check or CheckType.DoubleCheck => CheckType.CheckMate,
                _ => Game.CheckType,
            };
        }

        public override void Exit()
        {
            Game.Competitors.StopPlayer();
        }

        public override void Move(string uci)
        {
            Game.SetState(new MoveState(Game, uci));
        }

        public override void Undo()
        {
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                Game.SetState(new UndoState(Game, moveData));
            }
        }

        public override void Redo()
        {
            if (Game.UciBuffer.CanRedo(out MoveData moveData))
            {
                Game.SetState(new RedoState(Game, moveData));
            }
        }

        public override void Play()
        {
            // Already playing
        }

        public override void Pause()
        {
            Game.SetState(new PauseState(Game));
        }

        public override void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            Game.Competitors.UpdatePlayer();
        }
    }
}