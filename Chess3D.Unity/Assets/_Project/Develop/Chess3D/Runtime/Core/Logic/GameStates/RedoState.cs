using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic.Moves;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Sound;
using PurrNet.StateMachine;
using UnityEngine;

namespace Chess3D.Runtime.Core.Logic.GameStates
{
    public class RedoState : StateNode<MoveData>, IGameState<MoveData>
    {
        [Header("References")]
        [SerializeField] private Game game;

        private const float MoveTimeSec = 0.1f;
        private const float SoundT = 0.6f;

        private MoveData _moveData;
        private Turn _turn;
        private float _t;
        private bool _isRunning;

        public string Name => "Redo";

        public override void Enter(MoveData moveData)
        {
            Debug.Log("State: " + Name);

            _t = 0f;
            _moveData = moveData;

            ParsedUci parsedUci = GetParsedUci(_moveData.Uci);
            bool isValid = ValidateRedo(parsedUci);

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

            Square fromSquare = game.Board.GetSquare(from);
            Square toSquare = game.Board.GetSquare(to);
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
                _turn = new SimpleMove(game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType == MoveType.MovePromotion)
            {
                Piece promotedPiece = game.Board.GetSpawnedPiece(parsedUci.PromotedPieceType, game.CurrentTurnColor,
                    parsedUci.ToSquare);
                _turn = new MovePromotion(game, piece, parsedUci.FromSquare, parsedUci.ToSquare, promotedPiece);
                return true;
            }

            if (_moveData.MoveType is MoveType.Capture)
            {
                _moveData.BeatenPiece = parsedUci.ToSquare.GetPiece();
                _turn = new Capture(game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                    _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType is MoveType.EnPassant)
            {
                // No need to update _moveData.BeatenPiece because it cannot be promoted piece
                _turn = new Capture(game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                    _moveData.IsFirstMove);
                return true;
            }

            if (_moveData.MoveType is MoveType.CapturePromotion)
            {
                // Order matters. Must grab captured piece first
                _moveData.BeatenPiece = parsedUci.ToSquare.GetPiece();
                Piece promotedPiece = game.Board.GetSpawnedPiece(parsedUci.PromotedPieceType, game.CurrentTurnColor,
                    parsedUci.ToSquare);
                _turn = new CapturePromotion(game, piece, parsedUci.FromSquare, parsedUci.ToSquare,
                    promotedPiece, _moveData.BeatenPiece);
                return true;
            }

            if (_moveData.MoveType is MoveType.CastlingShort or MoveType.CastlingLong)
            {
                _turn = new Castling(game, _moveData.CastlingInfo, _moveData.IsFirstMove);
                return true;
            }

            return false;
        }

        private void AbortAndSetPreviousState()
        {
            Debug.Log("Invalid Redo");

            _isRunning = false;
            game.GameStateMachine.SetState(_moveData.PreviousState);
        }

        public override void Exit()
        {
            if (!_isRunning)
            {
                return;
            }

            _turn.End();

            game.ChangeTurn();
            game.UciBuffer.Redo();
            game.PreformCalculations();
            game.FireEndMove();

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
                game.GameStateMachine.SetState(_moveData.PreviousState);
            }
        }

        private bool IsProgressMove()
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
            if (game.IsCheck)
            {
                EffectsPlayer.Instance.PlayCheckSound();
            }
        }

        public void Move(string uci)
        {
            // Can't move
        }

        public void Undo()
        {
            // No need
        }

        public void Redo()
        {
            // Already Redo
        }

        public void Play()
        {
            // Not Paused
        }

        public void Pause()
        {
            // May be implemented, but with Undo and Redo
        }
    }
}