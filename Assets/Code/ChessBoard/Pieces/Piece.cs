using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Logic;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("gameManager")]
        [SerializeField] protected Game game;
        [SerializeField] protected Square currentSquare;

        public bool IsFirstMove { get; set; } = true;

        // All kind of squares

        public HashSet<Square> MoveSquares { get; private set; } = new();
        public Dictionary<Square, Piece> CaptureSquares { get; private set; } = new();
        public HashSet<Square> DefendSquares { get; } = new();
        public HashSet<Square> CannotMoveSquares { get; } = new();

        public void GetSectionAndAlign(Game game)
        {
            Construct(game);

            if (!Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hitInfo, 2f,
                    squareLayer))
            {
                return;
            }

            transform.position = hitInfo.transform.position;
            currentSquare = hitInfo.collider.GetComponent<Square>();
            currentSquare.SetPiece(this);
        }

        private void Construct(Game game)
        {
            this.game = game;
        }

        private void Start()
        {
            currentSquare.SetPiece(this);
        }

        public bool CanMoveTo(Square square)
        {
            return MoveSquares.Contains(square);
        }

        public bool CanEatAt(Square square, out Piece piece)
        {
            return CaptureSquares.TryGetValue(square, out piece);;
        }

        public void AddToBoard()
        {
            game.AddPiece(this);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Remove from game manager and destroy
        /// </summary>
        public void RemoveFromBoard()
        {
            game.RemovePiece(this);
            gameObject.SetActive(false);
        }

        public void MoveToBeaten()
        {
            transform.position = new Vector3(-1.5f, 0f, 0f);
            currentSquare.SetPiece(null);
            currentSquare = null;
            game.RemovePiece(this);
        }

        public void RemoveFromBeaten(Square square)
        {
            transform.position = square.transform.position;
            square.SetPiece(this);
            currentSquare = square;
            game.AddPiece(this);
        }

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

        /// <summary>
        /// Calculate all constrains after captures and moves are calculated
        /// </summary>
        public virtual void CalculateConstrains()
        {
            // Only calculate pins
            if (game.CheckType == CheckType.None)
            {
                if (!game.AttackLines.Contains(currentSquare, isCheck: false))
                {
                    return;
                }

                if (!game.AttackLines.TryGetAttackLine(this, out AttackLine attackLine))
                {
                    return;
                }

                CalculatePin(attackLine);
            }
            // Can move king, capture attacker or block attack line
            else if (game.CheckType == CheckType.Check)
            {
                if (game.AttackLines.TryGetAttackLine(this, out AttackLine attackLine)
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
            else if (game.CheckType == CheckType.DoubleCheck)
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
                if (!game.AttackLines.Contains(moveSquare, isCheck: true))
                {
                    tempMoveSquares.Remove(moveSquare);
                    CannotMoveSquares.Add(moveSquare);
                }
            }

            MoveSquares = tempMoveSquares;

            // Update capture
            var tempCaptureSquares = new Dictionary<Square, Piece>(CaptureSquares);
            foreach ((Square captureSquare, Piece _) in CaptureSquares)
            {
                Piece capturePiece = captureSquare.GetPiece();
                if (!game.AttackLines.ContainsAttacker(capturePiece, isCheck: true))
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
            var tempCaptureSquares = new Dictionary<Square, Piece>(CaptureSquares);
            foreach ((Square captureSquare, Piece _) in CaptureSquares)
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
            GetSectionAndAlign(FindObjectOfType<Game>());
        }

#endif
    }
}