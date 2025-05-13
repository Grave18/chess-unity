using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Info;
using ChessGame.ChessBoard.Pieces;
using ChessGame.Logic.Moves;
using ChessGame.Logic.MovesBuffer;
using Notation;
using UnityEngine;

namespace ChessGame.Logic.GameStates
{
    public class MoveState : GameState
    {
        private const float MoveTime = 0.3f;
        private readonly string _uci;

        private bool _isRunning;
        private Turn _turn;
        private MoveData _moveData;
        private float _t = 0;

        public MoveState(Game game, string uci) : base(game)
        {
            _uci = uci;
        }

        public override string Name => "Move";

        public override void Enter()
        {
            ParsedUci parsedUci = GetParsedUci(_uci);
            bool isValid = ValidateMove(parsedUci);

            if (isValid)
            {
                Run();
            }
            else
            {
                Abort();
            }
        }

        private ParsedUci GetParsedUci(string uci)
        {
            // Extract move form string
            string from = uci.Substring(0, 2);
            string to = uci.Substring(2, 2);

            Square fromSquare = Game.Board.GetSquare(from);
            Square toSquare = Game.Board.GetSquare(to);
            var promotedPieceType = PieceType.None;

            if (uci.Length == 5)
            {
                string promotion = uci.Substring(4, 1);
                promotedPieceType = Board.GetPieceType(promotion);
            }

            var parsedUci = new ParsedUci
            {
                FromSquare = fromSquare,
                ToSquare = toSquare,
                PromotedPieceType = promotedPieceType,
            };

            return parsedUci;
        }

        private bool ValidateMove(ParsedUci parsedUci)
        {
            Piece piece = parsedUci.FromSquare.GetPiece();

            _moveData = new MoveData
            {
                Uci = _uci,
                IsFirstMove = piece.IsFirstMove,
            };

            bool isValid = false;

            Piece promotedPiece = null;
            // Move
            if (piece.CanMoveTo(parsedUci.ToSquare, out MoveInfo moveInfo))
            {
                _moveData.EpSquareAddress = moveInfo.EnPassantSquare?.Address ?? "-";

                if (parsedUci.PromotedPieceType == PieceType.None)
                {
                    _moveData.MoveType = MoveType.Move;
                    _turn = new SimpleMove(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                }
                else
                {
                    _moveData.MoveType = MoveType.MovePromotion;
                    _moveData.HiddenPawn = piece;
                    promotedPiece = Game.Board.CreatePiece(parsedUci.PromotedPieceType, Game.CurrentTurnColor,
                        parsedUci.ToSquare);
                    _turn = new MovePromotion(_moveData.HiddenPawn, parsedUci.FromSquare, parsedUci.ToSquare,
                        promotedPiece);
                }

                isValid = true;
            }

            // Capture
            else if (piece.CanCaptureAt(parsedUci.ToSquare, out CaptureInfo captureInfo))
            {
                _moveData.BeatenPiece = captureInfo.BeatenPiece;
                if (parsedUci.PromotedPieceType == PieceType.None)
                {
                    _moveData.MoveType = captureInfo.MoveType;
                    _turn = new Capture(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                        _moveData.IsFirstMove);
                }
                else
                {
                    _moveData.MoveType = MoveType.CapturePromotion;
                    _moveData.HiddenPawn = piece;
                    promotedPiece = Game.Board.CreatePiece(parsedUci.PromotedPieceType, Game.CurrentTurnColor,
                        parsedUci.ToSquare);
                    _turn = new CapturePromotion(piece, parsedUci.FromSquare, parsedUci.ToSquare, promotedPiece,
                        _moveData.BeatenPiece);
                }

                isValid = true;
            }

            // Castling
            else if (piece is King king && king.CanCastlingAt(parsedUci.ToSquare, out CastlingInfo castlingInfo))
            {
                _moveData.MoveType = castlingInfo.MoveType;
                _moveData.CastlingInfo = castlingInfo;
                _turn = new Castling(_moveData.CastlingInfo, _moveData.IsFirstMove);
                isValid = true;
            }

            _moveData.TurnColor = Game.CurrentTurnColor;
            _moveData.AlgebraicNotation = Algebraic.Get(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.MoveType, promotedPiece);
            return isValid;
        }

        private void Run()
        {
            _isRunning = true;
        }

        private void Abort()
        {
            Debug.Log("Invalid move");
            Game.SetState(new IdleState(Game));
        }

        public override void Exit(string nextState)
        {
            // Empty
        }

        public override void Move(string uci)
        {
            // Empty
        }

        public override void Undo()
        {
            // Not Undo
        }

        public override void Redo()
        {
            // Not Redo
        }

        public override void Play()
        {
            // It is not paused
        }

        public override void Pause()
        {
            // Maybe implement pause from Move
        }

        public override void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            if (IsProgressMove())
            {
                ProgressMove();
            }
            else
            {
                EndMove();
            }
        }

        private bool IsProgressMove()
        {
            return _t < 1;
        }

        private void ProgressMove()
        {
            _t += Time.deltaTime / MoveTime;

            _turn.Progress(_t);
        }

        private void EndMove()
        {
            _turn.End();

            Game.ChangeTurn();
            Game.UciBuffer.Add(_moveData);
            Game.PreformCaluculations();
            UpdateAlgebraicNotation();
            Game.FireEndMove();
            Game.SetState(new IdleState(Game));
        }

        private void UpdateAlgebraicNotation()
        {
            _moveData.AlgebraicNotation += Algebraic.GetCheck(Game.CheckType);
        }
    }
}