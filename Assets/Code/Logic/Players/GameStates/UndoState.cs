using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.MovesBuffer;
using Logic.Players.Moves;
using UnityEngine;

namespace Logic.Players.GameStates
{
    public class UndoState : GameState
    {
        private readonly MoveData _moveData;
        private bool _isRunning;
        private Turn _turn;

        private float _t = 1;
        private const float TimeSec = 0.1f;

        public UndoState(Game game, MoveData moveData) : base(game)
        {
            _moveData = moveData;
        }

        public override string Name => "Undo";

        public override void Enter()
        {
            ParsedUci parsedUci = GetParsedUci(_moveData.Uci);
            bool isValid = ValidateUndo(parsedUci);

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

        private bool ValidateUndo(ParsedUci parsedUci)
        {
            Piece piece = parsedUci.ToSquare.GetPiece();

            if (_moveData.MoveType == MoveType.Move)
            {
                _turn = new SimpleMove(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType == MoveType.MovePromotion)
            {
                // Piece and Promoted piece swapped
                Piece promotedPiece = parsedUci.ToSquare.GetPiece();
                _turn = new MovePromotion(promotedPiece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.HiddenPawn);
                return true;
            }

            if (_moveData.MoveType is MoveType.Capture or MoveType.EnPassant)
            {
                _turn = new Capture(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                    _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType is MoveType.CapturePromotion)
            {
                Piece promotedPiece = parsedUci.ToSquare.GetPiece();
                // Piece and Promoted piece swapped
                _turn = new CapturePromotion(promotedPiece, parsedUci.FromSquare, parsedUci.ToSquare,
                    _moveData.HiddenPawn, _moveData.BeatenPiece);
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
            Debug.LogError("Invalid Undo");
            Game.SetPreviousState();
        }

        public override void Exit(string nextState)
        {
            // Empty
        }

        public override void Move(string uci)
        {
            // No need
        }

        public override void Undo()
        {
            // Already Undo
        }

        public override void Redo()
        {
            // No need
        }

        public override void Play()
        {
            // Not paused
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
            return _t > 0;
        }

        private void ProgressMove()
        {
            _t -= Time.deltaTime / TimeSec;

            _turn.Progress(_t);
        }

        private void EndMove()
        {
            _turn.EndUndo();
            Game.UciBuffer.Undo();

            Game.ChangeTurn();
            Game.Calculate();
            Game.FireEndMove();
            Game.SetPreviousState();
        }
    }
}