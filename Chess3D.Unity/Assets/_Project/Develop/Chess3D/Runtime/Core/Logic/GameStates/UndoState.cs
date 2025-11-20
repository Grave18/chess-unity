using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic.Moves;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Logic.Players;
using Chess3D.Runtime.Core.Sound;
using PurrNet.StateMachine;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class UndoState : StateNode<MoveData>, IGameState<MoveData>
    {
        private Game _game;
        private Board _board;
        private UciBuffer _uciBuffer;
        private IGameStateMachine _gameStateMachine;
        private CoreEvents _coreEvents;

        private const float MoveTimeSec = 0.1f;
        private const float SoundT = 0.4f;

        private MoveData _moveData;
        private Turn _turn;
        private float _t;
        private bool _isRunning;

        [Inject]
        public void Construct(Game game, Board board, UciBuffer uciBuffer, IGameStateMachine gameStateMachine, CoreEvents coreEvents)
        {
            _game = game;
            _board = board;
            _uciBuffer = uciBuffer;
            _gameStateMachine = gameStateMachine;
            _coreEvents = coreEvents;
        }

        public string Name => "Undo";

        public override void Enter(MoveData moveData)
        {
            Debug.Log("State: " + Name);

            _t = 1f;
            _moveData = moveData;

            ParsedUci parsedUci = GetParsedUci(_moveData.Uci);
            bool isValid = ValidateUndo(parsedUci);

            if (isValid)
            {
                _isRunning = true;
            }
            else
            {
                AbortAndSetPreviousState();
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
                _turn = new SimpleMove(_game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType == MoveType.MovePromotion)
            {
                // Piece and Promoted piece swapped
                Piece promotedPiece = parsedUci.ToSquare.GetPiece();
                _turn = new MovePromotion(_game, promotedPiece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.HiddenPawn);
                return true;
            }

            if (_moveData.MoveType is MoveType.Capture or MoveType.EnPassant)
            {
                _turn = new Capture(_game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                    _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType is MoveType.CapturePromotion)
            {
                Piece promotedPiece = parsedUci.ToSquare.GetPiece();
                // Piece and Promoted piece swapped
                _turn = new CapturePromotion(_game, promotedPiece, parsedUci.FromSquare, parsedUci.ToSquare,
                    _moveData.HiddenPawn, _moveData.BeatenPiece);
                return true;
            }

            if (_moveData.MoveType is MoveType.CastlingShort or MoveType.CastlingLong)
            {
                _turn = new Castling(_game, _moveData.CastlingInfo, _moveData.IsFirstMove);
                return true;
            }

            return false;
        }

        private void AbortAndSetPreviousState()
        {
            Debug.Log("Invalid Undo");
            _isRunning = false;
            _gameStateMachine.SetState(_moveData.PreviousState);
        }

        public override void Exit()
        {
            if (!_isRunning)
            {
                return;
            }

            _turn.EndUndo();

            _game.ChangeTurn();
            _uciBuffer.Undo();
            _game.PreformCalculations();
            _coreEvents.FireEndMove();

            PlayCheckSoundIfCheck();
        }

        public override void StateUpdate()
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
                _gameStateMachine.SetState(_moveData.PreviousState);
            }
        }

        private bool IsProgressMove()
        {
            return _t > 0;
        }

        private void ProgressMove()
        {
            float delta = Time.deltaTime / MoveTimeSec;
            _t -= delta;

            _turn.Progress(_t);
            PlaySound(delta);
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
            // No need
        }

        public void Undo()
        {
            // Already Undo
        }

        public void Redo()
        {
            // No need
        }

        public void Play()
        {
            // Not paused
        }

        public void Pause()
        {
            // May be implemented, but with Undo and Redo
        }
    }
}