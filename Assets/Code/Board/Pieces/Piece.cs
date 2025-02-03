using System.Collections.Generic;
using DG.Tweening;
using Logic;
using UnityEngine;

#pragma warning disable CS0252, CS0253

namespace Board.Pieces
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
        public bool IsFirstMove = true;
        [SerializeField] protected GameManager gameManager;

        [SerializeField] protected Square currentSquare;

        // All kind of squares

        public HashSet<Square> MoveSquares { get; private set; } = new();
        public HashSet<Square> CaptureSquares { get; private set; } = new();
        public HashSet<Square> DefendSquares { get; } = new();
        public HashSet<Square> CannotMoveSquares { get; } = new();

        public void GetSectionAndAlign(GameManager gameManager)
        {
            Construct(gameManager);

            if (!Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hitInfo, 2f,
                    squareLayer))
            {
                return;
            }

            transform.position = hitInfo.transform.position;
            currentSquare = hitInfo.collider.GetComponent<Square>();
            currentSquare.SetPiece(this);
        }

        private void Construct(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        private void Start()
        {
            currentSquare.SetPiece(this);
        }

        public bool CanMoveTo(Square square)
        {
            if (!gameManager.Selected.IsEqual(this) || !gameManager.IsRightTurn(pieceColor))
            {
                return false;
            }

            return MoveSquares.Contains(square);
        }

        public bool CanEatAt(Square square)
        {
            if (!gameManager.Selected.IsEqual(this) || !gameManager.IsRightTurn(pieceColor))
            {
                return false;
            }

            return CaptureSquares.Contains(square);
        }

        public Piece EatAt(Square square)
        {
            Piece piece = square.GetPiece();
            piece?.MoveToBeaten();

            return piece;
        }

        public void MoveTo(Square square)
        {
            Vector3 position = square.transform.position;

            Move(position);

            // Reset current square
            currentSquare.SetPiece(null);
            currentSquare = square;
            currentSquare.SetPiece(this);
        }

        private void MoveToBeaten()
        {
            transform.position = new Vector3(-1.5f, 0f, 0f);
            currentSquare.SetPiece(null);
            currentSquare = null;
            gameManager.RemovePiece(this);
        }

        public void RemoveFromBeaten(Square square)
        {
            transform.position = square.transform.position;
            square.SetPiece(this);
            currentSquare = square;
            gameManager.AddPiece(this);
        }

        private void Move(Vector3 position, TweenCallback callback = null)
        {
            // Move
            float distance = Vector3.Distance(transform.position, position);
            float duration = distance / animationSpeed;
            var tween = transform.DOMove(position, duration).SetEase(animationEase);

            if (callback != null)
            {
                tween.onComplete = callback;
            }
        }

        public virtual void CalculateConstrains()
        {
            if (gameManager.CheckType == CheckType.None)
            {
                if (!gameManager.AttackLines.Contains(currentSquare, isCheck: false))
                {
                    return;
                }

                if (!gameManager.AttackLines.TryGetAttackLine(this, out AttackLine attackLine))
                {
                    return;
                }

                CalculatePin(attackLine);
            }
            else if (gameManager.CheckType == CheckType.Check)
            {
                if (gameManager.AttackLines.TryGetAttackLine(this, out AttackLine attackLine)
                    && !attackLine.IsCheck)
                {
                    DisableMovesAndCaptures();
                }
                else
                {
                    UpdateMovesAndCaptures();
                }
            }
            else if (gameManager.CheckType == CheckType.DoubleCheck)
            {
                DisableMovesAndCaptures();
            }
        }

        private void DisableMovesAndCaptures()
        {
            // Update move
            foreach (Square moveSquare in MoveSquares)
            {
                CannotMoveSquares.Add(moveSquare);
            }

            MoveSquares.Clear();

            // Update capture
            CaptureSquares.Clear();
        }

        private void UpdateMovesAndCaptures()
        {
            // Update move
            var tempMoveSquares = new HashSet<Square>(MoveSquares);
            foreach (Square moveSquare in MoveSquares)
            {
                if (!gameManager.AttackLines.Contains(moveSquare, isCheck: true))
                {
                    tempMoveSquares.Remove(moveSquare);
                    CannotMoveSquares.Add(moveSquare);
                }
            }

            MoveSquares = tempMoveSquares;

            // Update capture
            var tempCaptureSquares = new HashSet<Square>(CaptureSquares);
            foreach (Square captureSquare in CaptureSquares)
            {
                Piece capturePiece = captureSquare.GetPiece();
                if (!gameManager.AttackLines.ContainsAttacker(capturePiece, isCheck: true))
                {
                    tempCaptureSquares.Remove(captureSquare);
                }
            }

            CaptureSquares = tempCaptureSquares;
        }

        private void CalculatePin(AttackLine attackLine)
        {
            var tempMoveSquares = new HashSet<Square>(MoveSquares);

            foreach (Square moveSquare in MoveSquares)
            {
                if (!attackLine.Contains(moveSquare))
                {
                    tempMoveSquares.Remove(moveSquare);
                    CannotMoveSquares.Add(moveSquare);
                }
            }

            MoveSquares = tempMoveSquares;

            // Update captures
            var tempCaptureSquares = new HashSet<Square>(CaptureSquares);
            foreach (Square captureSquare in CaptureSquares)
            {
                if (captureSquare.GetPiece() != attackLine.Attacker)
                {
                    tempCaptureSquares.Remove(captureSquare);
                }
            }

            CaptureSquares = tempCaptureSquares;
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

#if UNITY_EDITOR

        [ContextMenu("Get Section And Align")]
        private void GetSectionAndAlign()
        {
            GetSectionAndAlign(FindObjectOfType<GameManager>());
        }

#endif
    }
}