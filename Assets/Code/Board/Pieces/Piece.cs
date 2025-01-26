using System.Collections.Generic;
using DG.Tweening;
using Logic;
using UnityEngine;
#pragma warning disable CS0252, CS0253

namespace Board.Pieces
{
    public abstract class Piece : MonoBehaviour, ISelectable
    {
        [Header("Piece")]
        [SerializeField] protected PieceColor pieceColor;
        [SerializeField] protected GameManager gameManager;
        [SerializeField] protected Square currentSquare;
        [SerializeField] private LayerMask squareLayer;

        public bool IsFirstMove = true;
        [SerializeField] private List<Square> underAttackSquares;
        [SerializeField] private List<Square> moveSquares;
        [SerializeField] private List<Square> captureSquares;
        [SerializeField] private List<Square> cannotMoveSquares;

        [Header("Animation")]
        [SerializeField] private float animationSpeed = 1f;
        [SerializeField] private Ease animationEase = Ease.InOutCubic;

        // Getters
        public List<Square> UnderAttackSquares => underAttackSquares;
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

            return CanMoveToInternal(square);
        }

        public bool CanEatAt(Square square)
        {
            if (!gameManager.Selected.IsEqual(this) || !gameManager.IsRightTurn(pieceColor))
            {
                return false;
            }

            return CanEatAtInternal(square);
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

        public abstract void CalculateMovesAndCaptures();

        public void CalculateUnderAttackSquares()
        {
            // If piece is beaten cancel calculations
            if (currentSquare == null)
            {
                return;
            }

            CalculateUnderAttackSquaresInternal();
        }

        protected abstract void CalculateUnderAttackSquaresInternal();

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

        protected abstract bool CanEatAtInternal(Square square);
        protected abstract bool CanMoveToInternal(Square square);

        [ContextMenu("Get Section And Align")]
        public void GetSectionAndAlign()
        {
            Construct(FindObjectOfType<GameManager>());

            if (!Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hitInfo, 2f,
                                 squareLayer))
            {
                return;
            }

            transform.position = hitInfo.transform.position;
            currentSquare = hitInfo.collider.GetComponent<Square>();
            currentSquare.SetPiece(this);
        }
    }
}
