using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.Moves;
using Logic.MovesBuffer;
using Notation;
using PurrNet.StateMachine;
using Sound;
using UnityEngine;

namespace Logic.GameStates
{
    public class MoveState : StateNode<string>, IState
    {
        public string Name => "Move";
        protected Game Game { get; private set; }

        private const float MoveTimeSec = 0.3f;
        private readonly string _uci;

        private bool _isRunning;
        private MoveData _moveData;

        protected Turn Turn;
        protected float T { get; set; } = 0f;
        protected float SoundT => 0.6f;

        public MoveState(Game game, string uci)
        {
            Game = game;
            _uci = uci;
        }

        public override void Enter(string uci)
        {
            ParsedUci parsedUci = GetParsedUci(_uci);
            bool isValid = ValidateMove(parsedUci);

            if (isValid)
            {
                Run();
            }
            else
            {
                Abort(_uci);
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
                    Turn = new SimpleMove(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.IsFirstMove);
                }
                else
                {
                    _moveData.MoveType = MoveType.MovePromotion;
                    _moveData.HiddenPawn = piece;
                    promotedPiece = Game.Board.GetSpawnedPiece(parsedUci.PromotedPieceType, Game.CurrentTurnColor,
                        parsedUci.ToSquare);
                    Turn = new MovePromotion(Game,_moveData.HiddenPawn, parsedUci.FromSquare, parsedUci.ToSquare,
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
                    Turn = new Capture(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.BeatenPiece,
                        _moveData.IsFirstMove);
                }
                else
                {
                    _moveData.MoveType = MoveType.CapturePromotion;
                    _moveData.HiddenPawn = piece;
                    promotedPiece = Game.Board.GetSpawnedPiece(parsedUci.PromotedPieceType, Game.CurrentTurnColor,
                        parsedUci.ToSquare);
                    Turn = new CapturePromotion(Game, piece, parsedUci.FromSquare, parsedUci.ToSquare, promotedPiece,
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
                Turn = new Castling(Game,_moveData.CastlingInfo, _moveData.IsFirstMove);
                isValid = true;
            }

            _moveData.TurnColor = Game.CurrentTurnColor;
            _moveData.AlgebraicNotation = Algebraic.Get(piece, parsedUci.FromSquare, parsedUci.ToSquare, _moveData.MoveType, promotedPiece);
            return isValid;
        }

        private int GetIncrementedHalfMovesCounter()
        {
            return Game.UciBuffer.HalfMoveClock + 1;
        }

        private void ResetHalfMoveClock()
        {
            _moveData.HalfMoveClock = 0;
        }

        private int GetFullMoveCounter(PieceColor pieceColor)
        {
            int counter = Game.UciBuffer.FullMoveCounter;

            if (pieceColor == PieceColor.Black)
            {
                return counter + 1;
            }

            return counter;
        }

        private void Run()
        {
            _isRunning = true;
        }

        private void Abort(string uci)
        {
            Debug.Log($"{uci}: invalid move");
            Game.GameStateMachine.SetState(new IdleState(Game));
        }

        public override void Exit()
        {
            // Empty
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
                EndMove();
            }
        }

        private bool IsProgressMove()
        {
            return T < 1;
        }

        private void ProgressMove()
        {
            float delta = Time.deltaTime / MoveTimeSec;
            T += delta;

            Turn.Progress(T);
            PlaySound(delta);
        }

        private void EndMove()
        {
            Turn.End();

            Game.ChangeTurn();
            Game.UciBuffer.Add(_moveData);
            Game.PreformCalculations();
            UpdateAlgebraicNotation();
            Game.FireEndMove();
            Game.GameStateMachine.SetState(new IdleState(Game));
            PlayCheckSound();
        }

        private void UpdateAlgebraicNotation()
        {
            _moveData.AlgebraicNotation += Algebraic.GetCheck(Game.CheckType);
        }

        protected void PlaySound(float delta)
         {
             // Not 0.5 sec because of duplicate sound
             float halfDelta = delta*0.525f;
             if (T >= SoundT - halfDelta && T <= SoundT + halfDelta)
             {
                 Turn.PlaySound();
             }
         }

         protected void PlayCheckSound()
         {
             if (Game.IsCheck)
             {
                 EffectsPlayer.Instance.PlayCheckSound();
             }
         }
    }
}