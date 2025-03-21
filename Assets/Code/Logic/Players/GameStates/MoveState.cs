using AlgebraicNotation;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.MovesBuffer;
using Logic.Players.Moves;
using UnityEngine;

namespace Logic.Players.GameStates
{
    public class MoveState : GameState
    {
        private readonly string _uci;
        private readonly bool _isRedo;

        private bool _isRunning;
        private Turn turn;
        private MoveData _moveData;
        private float _t;

        public MoveState(Game game, string uci, bool isRedo = false) : base(game)
        {
            _uci = uci;
            _isRedo = isRedo;
        }

        public override string Name => _isRedo ? "Redo" : "Move";

        public override void Enter()
        {
            ParsedUci parsedUci = GetParsedUci(_uci);
            bool isValid = Validate(parsedUci);

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
            Piece piece = fromSquare.GetPiece();
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

        private bool Validate(ParsedUci parsedUci)
        {
            Piece piece = parsedUci.Piece;

            _moveData = new MoveData
            {
                Uci = _uci,
                IsFirstMove = parsedUci.Piece.IsFirstMove
            };

            // Move
            if (piece.CanMoveTo(parsedUci.ToSquare, out MoveInfo moveInfo))
            {
                _moveData.MoveType = MoveType.Move;
                _moveData.EpSquareAddress = moveInfo.EnPassantSquare.Address;
                turn = new SimpleMove(parsedUci, _moveData);
                return true;
            }

            // Capture
            if (piece.CanCaptureAt(parsedUci.ToSquare, out CaptureInfo captureInfo))
            {
                _moveData.MoveType = captureInfo.MoveType;
                turn = new Capture(parsedUci, _moveData);
                return true;
            }

            // Castling
            if (piece is King king && king.CanCastlingAt(parsedUci.ToSquare, out CastlingInfo castlingInfo))
            {
                _moveData.MoveType = castlingInfo.MoveType;
                _moveData.CastlingInfo = castlingInfo;
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

            if (_t < 1)
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
            if (_isRedo)
            {
                _t += Time.deltaTime / 0.1f;
            }
            else
            {
                _t += Time.deltaTime / 0.3f;
            }

            turn.Progress(_t);
        }

        private void EndMove()
        {
            turn.End();

            if (_isRedo)
            {
                Game.CommandBuffer.Redo();
            }
            else
            {
                Game.CommandBuffer.Add(_moveData);
            }

            Game.ChangeTurn();
            Game.SetState(new IdleState(Game));
        }
    }
}