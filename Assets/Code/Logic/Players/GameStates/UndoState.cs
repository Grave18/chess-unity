using ChessBoard;
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
        private Turn turn;

        private float _t = 1;
        private const float TimeSec = 0.1f;

        public UndoState(Game game, MoveData moveData) : base(game)
        {
            _moveData = moveData;
        }

        public override string Name => "Undo";

        public override void Enter()
        {
            ParsedUci parsedUci = GetParsedUciUndo(_moveData.Uci);
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

        private ParsedUci GetParsedUciUndo(string uci)
        {
            // Extract move form string
            string from = uci.Substring(0, 2);
            string to = uci.Substring(2, 2);

            Square fromSquare = Game.Board.GetSquare(from);
            Square toSquare = Game.Board.GetSquare(to);
            Piece piece = toSquare.GetPiece();
            Piece promotedPiece = null;

            if (uci.Length == 5)
            {
                string promotion = uci.Substring(4, 1);
                promotedPiece = Game.Board.GetPiece(promotion, Game.CurrentTurnColor, to);
            }

            var parsedUci = new ParsedUci
            {
                Piece = piece,
                FromSquare = fromSquare,
                ToSquare = toSquare,
                PromotedPiece = promotedPiece,
            };

            return parsedUci;
        }

        private bool ValidateUndo(ParsedUci parsedUci)
        {
            if (_moveData.MoveType == MoveType.Move)
            {
                turn = new SimpleMove(parsedUci, _moveData);
                return true;
            }

            if (_moveData.MoveType is MoveType.Capture or MoveType.EnPassant)
            {
                turn = new Capture(parsedUci, _moveData);
                return true;
            }

            if (_moveData.MoveType is MoveType.CastlingShort or MoveType.CastlingLong)
            {
                turn = new Castling(parsedUci, _moveData);
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
            // Todo: maybe use stack of states
            Debug.Log("Invalid move");
            Game.SetState(new IdleState(Game));
        }

        public override void Exit()
        {
        }

        public override void Move(string uci)
        {
        }

        public override void Undo()
        {
        }

        public override void Redo()
        {
        }

        public override void Play()
        {
        }

        public override void Pause()
        {
        }

        public override void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            if (_t > 0)
            {
                ProgressMove();
            }
            else
            {
                EndMove();
            }
        }

        private void ProgressMove()
        {
            _t -= Time.deltaTime / TimeSec;

            turn.Progress(_t);
        }

        private void EndMove()
        {
            turn.EndUndo();

            Game.CommandBuffer.Undo();
            Game.ChangeTurn();
            Game.SetState(new IdleState(Game));
        }
    }
}