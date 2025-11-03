using System.Collections.Generic;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Utilities.Common.Mathematics;
using UnityEngine;

#pragma warning disable CS0252, CS0253

namespace Chess3D.Runtime.Core.ChessBoard.Pieces
{
    public abstract class Piece : MonoBehaviour, ISelectable
    {
        [Header("Piece settings")]
        [SerializeField] protected PieceColor pieceColor;

        [Header("Debug preview")]
        [SerializeField] protected Square currentSquare;
        [field:SerializeField] public bool IsFirstMove { get; set; }

        [Header("Piece movement settings")]
        [SerializeField] protected EasingType moveEasing = EasingType.InOutCubic;

        public Dictionary<Square, MoveInfo> MoveSquares { get; } = new();
        public Dictionary<Square, CaptureInfo> CaptureSquares { get; } = new();
        public HashSet<Square> DefendSquares { get; } = new();
        public HashSet<Square> CannotMoveSquares { get; } = new();

        // Initialize
        protected Game Game { get; private set; }
        protected Board Board { get; private set; }

        public void Init(Game game, Board board, Square square)
        {
            Game = game;
            Board = board;

            currentSquare = square;
            currentSquare.SetPiece(this);
        }

        public bool CanMoveTo(Square square, out MoveInfo moveInfo)
        {
            return MoveSquares.TryGetValue(square, out moveInfo);
        }

        public bool CanCaptureAt(Square square, out CaptureInfo captureInfo)
        {
            return CaptureSquares.TryGetValue(square, out captureInfo);
        }

        public void RemoveFromBoard()
        {
            SetNewSquare(null);
            Board.RemovePiece(this);
        }

        public void AddToBoard(Square square)
        {
            SetNewSquare(square);
            transform.position = square.transform.position;
            gameObject.SetActive(true);
            Board.AddPiece(this);
        }

        public void SetNewSquare(Square square)
        {
            currentSquare?.SetPiece(null);
            currentSquare = square;
            currentSquare?.SetPiece(this);
        }

        public virtual void MoveTo(Vector3 from, Vector3 to, float t)
        {
            Vector3 newPosition = Vector3.Lerp(from, to, Easing.Generic(t, moveEasing));
            transform.position = newPosition;
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