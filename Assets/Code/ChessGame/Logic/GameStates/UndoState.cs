using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Info;
using ChessGame.ChessBoard.Pieces;
using ChessGame.Logic.Moves;
using ChessGame.Logic.MovesBuffer;
using UnityEngine;

namespace ChessGame.Logic.GameStates
{
    public class UndoState : MovableState
    {
        private const float MoveTimeSec = 0.1f;

        private readonly MoveData _moveData;
        private bool _isRunning;

        public override string Name => "Undo";
        protected override float T { get; set; } = 1f;
        protected override float SoundT => 0.4f;

        public UndoState(Game game, MoveData moveData) : base(game)
        {
            _moveData = moveData;
        }

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
                Turn = new SimpleMove(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType == MoveType.MovePromotion)
            {
                // Piece and Promoted piece swapped
                Piece promotedPiece = parsedUci.ToSquare.GetPiece();
                Turn = new MovePromotion(Game, promotedPiece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.HiddenPawn);
                return true;
            }

            if (_moveData.MoveType is MoveType.Capture or MoveType.EnPassant)
            {
                Turn = new Capture(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                    _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType is MoveType.CapturePromotion)
            {
                Piece promotedPiece = parsedUci.ToSquare.GetPiece();
                // Piece and Promoted piece swapped
                Turn = new CapturePromotion(Game, promotedPiece, parsedUci.FromSquare, parsedUci.ToSquare,
                    _moveData.HiddenPawn, _moveData.BeatenPiece);
                return true;
            }

            if (_moveData.MoveType is MoveType.CastlingShort or MoveType.CastlingLong)
            {
                Turn = new Castling(Game, _moveData.CastlingInfo, _moveData.IsFirstMove);
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
            Game.Machine.SetPreviousState();
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
            return T > 0;
        }

        private void ProgressMove()
        {
            float delta = Time.deltaTime / MoveTimeSec;
            T -= delta;

            Turn.Progress(T);
            PlaySound(delta);
        }

        private void EndMove()
        {
            Turn.EndUndo();

            Game.ChangeTurn();
            Game.UciBuffer.Undo();
            Game.PreformCalculations();
            Game.FireEndMove();
            Game.Machine.SetPreviousState();

            PlayCheckSound();
        }
    }
}