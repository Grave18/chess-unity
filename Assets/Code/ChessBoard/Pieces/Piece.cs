using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChessBoard.Info;
using DG.Tweening;
using Logic;
using UnityEngine;

#pragma warning disable CS0252, CS0253

namespace ChessBoard.Pieces
{
    public abstract class Piece : MonoBehaviour, ISelectable
    {
        [Header("Piece settings")]
        [SerializeField] protected PieceColor pieceColor;

        [SerializeField] private LayerMask squareLayer;

        [Header("Animation")]
        [SerializeField] private float animationSpeed = 1f;
        [SerializeField] private Ease animationEase = Ease.InOutCubic;

        [Header("Debug preview")]
        [SerializeField] protected Square currentSquare;
        [field:SerializeField] public bool IsFirstMove { get; set; }

        public Dictionary<Square, MoveInfo> MoveSquares { get; } = new();
        public Dictionary<Square, CaptureInfo> CaptureSquares { get; } = new();
        public HashSet<Square> DefendSquares { get; } = new();
        public HashSet<Square> CannotMoveSquares { get; } = new();

        // Initialize
        protected Game Game { get; private set; }
        private Board _board;
        private BeatenPiecesPlace _beatenPiecesPlace;

        public void Init(Game game, Board board, Square square, BeatenPiecesPlace beatenPiecesPlace)
        {
            Game = game;
            _board = board;
            _beatenPiecesPlace = beatenPiecesPlace;

            currentSquare = square;
            currentSquare.SetPiece(this);
        }

        public bool CanMoveTo(Square square, out MoveInfo moveInfo)
        {
            return MoveSquares.TryGetValue(square, out moveInfo);
        }

        public bool CanEatAt(Square square, out CaptureInfo captureInfo)
        {
            return CaptureSquares.TryGetValue(square, out captureInfo);
        }

        public void MoveToBeaten()
        {
            _beatenPiecesPlace.Add(this);
            currentSquare.SetPiece(null);
            currentSquare = null;
            _board.RemovePiece(this);
        }

        public void RemoveFromBeaten(Square square)
        {
            _beatenPiecesPlace.Remove(this, square.transform.position);
            square.SetPiece(this);
            currentSquare = square;
            _board.AddPiece(this);
        }

        // Todo: remove this
        public async Task MoveToAsync(Square square)
        {
            Vector3 position = square.transform.position;

            // Move
            float distance = Vector3.Distance(transform.position, position);
            float duration = distance / animationSpeed;

            Tween moveTween = transform
                .DOMove(position, duration)
                .SetEase(animationEase);

            // Reset current square
            currentSquare.SetPiece(null);
            currentSquare = square;
            currentSquare.SetPiece(this);

            await moveTween.AsyncWaitForCompletion();
        }

        public void MoveTo(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        public void ResetSquare(Square square)
        {
            currentSquare.SetPiece(null);
            currentSquare = square;
            currentSquare.SetPiece(this);
        }

        /// Calculate all constrains after captures and moves are calculated
        public virtual void CalculateConstrains()
        {
            // Only calculate pins
            if (Game.CheckType == CheckType.None)
            {
                if (!Game.AttackLines.Contains(currentSquare, isCheck: false))
                {
                    return;
                }

                if (!Game.AttackLines.TryGetAttackLine(this, out AttackLine attackLine))
                {
                    return;
                }

                CalculatePin(attackLine);
            }
            // Can move king, capture attacker or block attack line
            else if (Game.CheckType == CheckType.Check)
            {
                if (Game.AttackLines.TryGetAttackLine(this, out AttackLine attackLine)
                    && !attackLine.IsCheck)
                {
                    DisableMovesAndCaptures();
                }
                else
                {
                    UpdateMovesAndCaptures();
                }
            }
            // Only king can move out of attack or capture threatening piece
            else if (Game.CheckType == CheckType.DoubleCheck)
            {
                DisableMovesAndCaptures();
            }
        }

        private void DisableMovesAndCaptures()
        {
            var tempMoveSquares = new List<Square>(MoveSquares.Keys);
            // Update move
            foreach (Square moveSquare in tempMoveSquares)
            {
                CannotMoveSquares.Add(moveSquare);
            }

            MoveSquares.Clear();
            CaptureSquares.Clear();
        }

        private void UpdateMovesAndCaptures()
        {
            // Update move
            var tempMoveSquares = new List<Square>(MoveSquares.Keys);

            foreach (Square moveSquare in tempMoveSquares)
            {
                if (!Game.AttackLines.Contains(moveSquare, isCheck: true))
                {
                    MoveSquares.Remove(moveSquare);
                    CannotMoveSquares.Add(moveSquare);
                }
            }

            // Update capture
            var tempCaptureSquares = new List<Square>(CaptureSquares.Keys);

            foreach (Square captureSquare in tempCaptureSquares)
            {
                Piece capturePiece = captureSquare.GetPiece();
                if (!Game.AttackLines.ContainsAttacker(capturePiece, isCheck: true))
                {
                    CaptureSquares.Remove(captureSquare);
                }
            }
        }

        private void CalculatePin(AttackLine attackLine)
        {
            var tempMoveSquares = new List<Square>(MoveSquares.Keys);

            foreach (Square moveSquare in tempMoveSquares)
            {
                if (!attackLine.Contains(moveSquare))
                {
                    MoveSquares.Remove(moveSquare);
                    CannotMoveSquares.Add(moveSquare);
                }
            }

            // Update captures
            var tempCaptureSquares = new List<Square>(CaptureSquares.Keys);
            foreach (Square captureSquare in tempCaptureSquares)
            {
                if (captureSquare.GetPiece() != attackLine.Attacker)
                {
                    CaptureSquares.Remove(captureSquare);
                }
            }
        }

        public void CalculateMovesAndCaptures()
        {
            // If piece is beaten cancel calculations
            if (currentSquare == null)
            {
                return;
            }

            MoveSquares.Clear();
            CaptureSquares.Clear();
            CannotMoveSquares.Clear();
            DefendSquares.Clear();

            CalculateMovesAndCapturesInternal();
        }

        protected abstract void CalculateMovesAndCapturesInternal();

        public Piece GetPiece()
        {
            return this;
        }

        public PieceColor GetPieceColor()
        {
            return pieceColor;
        }

        public Square GetSquare()
        {
            return currentSquare;
        }

        public bool IsSquare()
        {
            // Must be always false
            return false;
        }

        public bool IsEqual(ISelectable other)
        {
            return this == other || currentSquare == other;
        }

        public bool HasPiece()
        {
            return true;
        }
    }
}