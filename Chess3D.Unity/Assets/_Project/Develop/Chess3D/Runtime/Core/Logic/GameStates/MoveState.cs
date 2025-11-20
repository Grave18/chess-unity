using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic.Moves;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Core.Sound;
using PurrNet.StateMachine;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class MoveState : StateNode<string>, IGameState<string>
    {
        private Game _game;
        private Board _board;
        private PieceFactory _pieceFactory;
        private UciBuffer _uciBuffer;
        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;

        [SerializeField] private IdleState idleState;

        private const float MoveTimeSec = 0.3f;
        private const float SoundT = 0.6f;

        private string _uci;
        private MoveData _moveData;
        private Turn _turn;
        private float _t;
        private bool _isRunning;

        [Inject]
        public void Construct(Game game, Board board, PieceFactory pieceFactory, UciBuffer uciBuffer,
            IGameStateMachine gameStateMachine, CoreEvents coreEvents)
        {
            _game = game;
            _board = board;
            _pieceFactory = pieceFactory;
            _uciBuffer = uciBuffer;
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
        }

        public string Name => "Move";

        public override void Enter(string uci)
        {
            Debug.Log("State: " + Name);

            _t = 0f;
            _uci = uci;

            ParsedUci parsedUci = GetParsedUci(_uci);
            bool isValid = ValidateMove(parsedUci);

            if (isValid)
            {
                _isRunning = true;
            }
            else
            {
                AbortAndSetIdle(_uci);
            }
        }

        private ParsedUci GetParsedUci(string uci)
        {
            // Extract move form string
            string from = uci.Substring(0, 2);
            string to = uci.Substring(2, 2);

            Square fromSquare = _board.GetSquare(from);
            Square toSquare = _board.GetSquare(to);
            var promotedPieceType = PieceType.None;

            // If uci with promotion
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
                HalfMoveClock = GetIncrementedHalfMovesCounter(),
                FullMoveCounter = GetFullMoveCounter(piece.GetPieceColor()),
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
                    _turn = new SimpleMove(_game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                }
                else
                {
                    _moveData.MoveType = MoveType.MovePromotion;
                    _moveData.HiddenPawn = piece;
                    promotedPiece = _pieceFactory.Create(parsedUci.PromotedPieceType, _game.CurrentTurnColor,
                        parsedUci.ToSquare);
                    _turn = new MovePromotion(_game,_moveData.HiddenPawn, parsedUci.FromSquare, parsedUci.ToSquare,
                        promotedPiece);
                }

                // Reset rule 50 if pawn move
                if (piece is Pawn)
                {
                    ResetHalfMoveClock();
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
                    _turn = new Capture(_game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                        _moveData.IsFirstMove);
                }
                else
                {
                    _moveData.MoveType = MoveType.CapturePromotion;
                    _moveData.HiddenPawn = piece;
                    promotedPiece = _pieceFactory.Create(parsedUci.PromotedPieceType, _game.CurrentTurnColor,
                        parsedUci.ToSquare);
                    _turn = new CapturePromotion(_game, piece, parsedUci.FromSquare, parsedUci.ToSquare, promotedPiece,
                        _moveData.BeatenPiece);
                }

                // Reset rule 50 if capture move
                ResetHalfMoveClock();

                isValid = true;
            }

            // Castling
            else if (piece is King king && king.CanCastlingAt(parsedUci.ToSquare, out CastlingInfo castlingInfo))
            {
                _moveData.MoveType = castlingInfo.MoveType;
                _moveData.CastlingInfo = castlingInfo;
                _turn = new Castling(_game,_moveData.CastlingInfo, _moveData.IsFirstMove);
                isValid = true;
            }

            _moveData.TurnColor = _game.CurrentTurnColor;
            _moveData.AlgebraicNotation = Algebraic.Get(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.MoveType, promotedPiece);
            return isValid;
        }

        private int GetIncrementedHalfMovesCounter()
        {
            return _uciBuffer.HalfMoveClock + 1;
        }

        private void ResetHalfMoveClock()
        {
            _moveData.HalfMoveClock = 0;
        }

        private int GetFullMoveCounter(PieceColor pieceColor)
        {
            int counter = _uciBuffer.FullMoveCounter;

            if (pieceColor == PieceColor.Black)
            {
                return counter + 1;
            }

            return counter;
        }

        private void AbortAndSetIdle(string uci)
        {
            Debug.Log($"{uci}: invalid move");
            _isRunning = false;
            _gameStateMachine.SetState(idleState);
        }

        public override void Exit()
        {
            if (_isRunning)
            {
                _turn.Progress(1);
                _turn.End();

                _game.ChangeTurn();
                _uciBuffer.Add(_moveData);
                _game.PreformCalculations();
                UpdateAlgebraicNotation();
                PlayCheckSoundIfCheck();
            }

            _coreEvents.FireEndMove();
        }

        public override void StateUpdate()
        {
            if (!_isRunning)
            {
                return;
            }

            if (CanProgressMove())
            {
                ProgressMove();
            }
            else
            {
                _gameStateMachine.SetState(idleState);
            }
        }

        private bool CanProgressMove()
        {
            return _t < 1;
        }

        private void ProgressMove()
        {
            float delta = Time.deltaTime / MoveTimeSec;
            _t += delta;

            _turn.Progress(_t);
            PlaySound(delta);
        }

        private void UpdateAlgebraicNotation()
        {
            _moveData.AlgebraicNotation += Algebraic.GetCheck(_game.CheckType);
        }

        private void PlaySound(float delta)
         {
             // Not 0.5 sec because of duplicate sound
             float halfDelta = delta*0.525f;
             if (_t >= SoundT - halfDelta && _t <= SoundT + halfDelta)
             {
                 _turn.PlaySound();
             }
         }

        private void PlayCheckSoundIfCheck()
        {
            if (_game.IsCheck)
            {
                EffectsPlayer.Instance.PlayCheckSound();
            }
        }

        public void Move(string uci)
        {
         // Empty
        }

        public void Undo()
        {
         // Not Undo
        }

        public void Redo()
        {
         // Not Redo
        }

        public void Play()
        {
         // It is not paused
        }

        public void Pause()
        {
         // Maybe implement pause from Move
         }
    }
}