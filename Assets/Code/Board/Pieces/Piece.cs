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
        [SerializeField] protected List<Square> moveSquares;
        [SerializeField] protected List<Square> captureSquares;
        [SerializeField] protected List<Square> cannotMoveSquares;

        // Getters
        public List<Square> MoveSquares => moveSquares;
        public List<Square> CaptureSquares => captureSquares;
        public List<Square> CannotMoveSquares => cannotMoveSquares;

        public void Construct(GameManager gameManager)
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
        }

        public void RemoveFromBeaten(Square square)
        {
            transform.position = square.transform.position;
            currentSquare = square;
            square.SetPiece(this);
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
            if(gameManager.CheckType == CheckType.None)
            {
                return;
            }

            // This constrains only if check
            // Update move
            var tempMoveSquares = new List<Square>(moveSquares);
            foreach (Square moveSquare in moveSquares)
            {
                if (!gameManager.AttackLine.Contains(moveSquare))
                {
                    tempMoveSquares.Remove(moveSquare);
                    cannotMoveSquares.Add(moveSquare);
                }
            }
            moveSquares = tempMoveSquares;

            // Update capture
            var tempCaptureSquares = new List<Square>(captureSquares);
            foreach (Square captureSquare in captureSquares)
            {
                Piece capturePiece = captureSquare.GetPiece();
                if (!gameManager.Attackers.Contains(capturePiece))
                {
                    tempCaptureSquares.Remove(captureSquare);
                }
            }
            captureSquares = tempCaptureSquares;

            // Can't do any if DoubleCheck
        }

        public void CalculateMovesAndCaptures()
        {
            // If piece is beaten cancel calculations
            if (currentSquare == null)
            {
                return;
            }

            moveSquares.Clear();
            captureSquares.Clear();
            cannotMoveSquares.Clear();

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

#if UNITY_EDITOR

        [ContextMenu("Get Section And Align")]
        private void GetSectionAndAlign()
        {
            GetSectionAndAlign(FindObjectOfType<GameManager>());
        }

#endif
    }
}
