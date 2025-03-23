using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.MovesBuffer;
using Logic.Players.Moves;
using UnityEngine;

namespace Logic.Players.GameStates
{
    public class RedoState : GameState
    {
        private readonly MoveData _moveData;
        private bool _isRunning;
        private Turn _turn;

        private float _t = 0;
        private const float TimeSec = 0.1f;

        public RedoState(Game game, MoveData moveData) : base(game)
        {
            _moveData = moveData;
        }

        public override string Name => "Redo";

        public override void Enter()
        {
            ParsedUci parsedUci = GetParsedUci(_moveData.Uci);
            bool isValid = ValidateRedo(parsedUci);

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

        private bool ValidateRedo(ParsedUci parsedUci)
        {
            Piece piece = parsedUci.FromSquare.GetPiece();

            if (_moveData.MoveType == MoveType.Move)
            {
                _turn = new SimpleMove(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType == MoveType.MovePromotion)
            {
                Piece promotedPiece = Game.Board.CreatePiece(parsedUci.PromotedPieceType, Game.CurrentTurnColor,
                    parsedUci.ToSquare);
                _turn = new MovePromotion(piece, parsedUci.FromSquare, parsedUci.ToSquare, promotedPiece);
                return true;
            }

            if (_moveData.MoveType is MoveType.Capture)
            {
                _moveData.BeatenPiece = parsedUci.ToSquare.GetPiece();
                _turn = new Capture(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                    _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType is MoveType.EnPassant)
            {
                // No need to update _moveData.BeatenPiece because it cannot be promoted piece
                _turn = new Capture(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                    _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType is MoveType.CapturePromotion)
            {
                // Order matters. Must grab captured piece first
                _moveData.BeatenPiece = parsedUci.ToSquare.GetPiece();
                Piece promotedPiece = Game.Board.CreatePiece(parsedUci.PromotedPieceType, Game.CurrentTurnColor,
                    parsedUci.ToSquare);
                _turn = new CapturePromotion(piece, parsedUci.FromSquare, parsedUci.ToSquare,
                    promotedPiece, _moveData.BeatenPiece);
                return true;
            }

            if (_moveData.MoveType is MoveType.CastlingShort or MoveType.CastlingLong)
            {
                _turn = new Castling(_moveData.CastlingInfo, _moveData.IsFirstMove);
                return true;
            }

            return false;
        }

        private void Run()
        {
            _isRunning = true;
        }

        private void Abort()
        {
            Debug.LogError("Invalid Redo");
            Game.SetPreviousState();
        }

        public override void Exit()
        {
            // Empty
        }

        public override void Move(string uci)
        {
            // Can't move
        }

        public override void Undo()
        {
            // No need
        }

        public override void Redo()
        {
            // Already Redo
        }

        public override void Play()
        {
            // Not Paused
        }

        public override void Pause()
        {
            // May be implemented, but with Undo and Redo
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
            _t += Time.deltaTime / TimeSec;

            _turn.Progress(_t);
        }

        private void EndMove()
        {
            _turn.End();

            Game.CommandBuffer.Redo();
            Game.ChangeTurn();
            Game.SetPreviousState();
        }
    }
}