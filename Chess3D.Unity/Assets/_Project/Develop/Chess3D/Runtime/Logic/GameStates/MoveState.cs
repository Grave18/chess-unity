using Chess3D.Runtime.ChessBoard;
using Chess3D.Runtime.ChessBoard.Info;
using Chess3D.Runtime.ChessBoard.Pieces;
using Chess3D.Runtime.Logic.Moves;
using Chess3D.Runtime.Logic.MovesBuffer;
using Chess3D.Runtime.Notation;
using Chess3D.Runtime.Sound;
using PurrNet.StateMachine;
using TNRD;
using UnityEngine;

namespace Chess3D.Runtime.Logic.GameStates
{
    public class MoveState : StateNode<string>, IGameState<string>
    {
        [Header("References")]
        [SerializeField] private Game game;

        [Header("States")]
        [SerializeField] private SerializableInterface<IGameState> idleState;

        private const float MoveTimeSec = 0.3f;
        private const float SoundT = 0.6f;

        private string _uci;
        private MoveData _moveData;
        private Turn _turn;
        private float _t;
        private bool _isRunning;

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

            Square fromSquare = game.Board.GetSquare(from);
            Square toSquare = game.Board.GetSquare(to);
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
                    _turn = new SimpleMove(game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                }
                else
                {
                    _moveData.MoveType = MoveType.MovePromotion;
                    _moveData.HiddenPawn = piece;
                    promotedPiece = game.Board.GetSpawnedPiece(parsedUci.PromotedPieceType, game.CurrentTurnColor,
                        parsedUci.ToSquare);
                    _turn = new MovePromotion(game,_moveData.HiddenPawn, parsedUci.FromSquare, parsedUci.ToSquare,
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
                    _turn = new Capture(game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                        _moveData.IsFirstMove);
                }
                else
                {
                    _moveData.MoveType = MoveType.CapturePromotion;
                    _moveData.HiddenPawn = piece;
                    promotedPiece = game.Board.GetSpawnedPiece(parsedUci.PromotedPieceType, game.CurrentTurnColor,
                        parsedUci.ToSquare);
                    _turn = new CapturePromotion(game, piece, parsedUci.FromSquare, parsedUci.ToSquare, promotedPiece,
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
                _turn = new Castling(game,_moveData.CastlingInfo, _moveData.IsFirstMove);
                isValid = true;
            }

            _moveData.TurnColor = game.CurrentTurnColor;
            _moveData.AlgebraicNotation = Algebraic.Get(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.MoveType, promotedPiece);
            return isValid;
        }

        private int GetIncrementedHalfMovesCounter()
        {
            return game.UciBuffer.HalfMoveClock + 1;
        }

        private void ResetHalfMoveClock()
        {
            _moveData.HalfMoveClock = 0;
        }

        private int GetFullMoveCounter(PieceColor pieceColor)
        {
            int counter = game.UciBuffer.FullMoveCounter;

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
            game.GameStateMachine.SetState(idleState.Value);
        }

        public override void Exit()
        {
            if (_isRunning)
            {
                _turn.Progress(1);
                _turn.End();

                game.ChangeTurn();
                game.UciBuffer.Add(_moveData);
                game.PreformCalculations();
                UpdateAlgebraicNotation();
                PlayCheckSoundIfCheck();
            }

            game.FireEndMove();
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
                game.GameStateMachine.SetState(idleState.Value);
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
            _moveData.AlgebraicNotation += Algebraic.GetCheck(game.CheckType);
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