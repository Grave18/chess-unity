using DG.Tweening;
using Logic;
using UnityEngine;

namespace Board
{
    public abstract class Piece : MonoBehaviour, ISelectable
    {
        private enum State
        {
            None, Selected, Highlighted
        }

        [SerializeField] protected PieceColor pieceColor;
        [SerializeField] protected GameManager gameManager;
        [SerializeField] protected Square currentSquare;

        [SerializeField] private LayerMask squareLayer;
        [SerializeField] private Ease ease = Ease.InOutCubic;
        [SerializeField] private float speed = 0.7f;

        private MeshRenderer _renderer;
        private State _state = State.None;

        public void Construct(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            currentSquare.SetPiece(this);
        }

        public void Select()
        {
            if (_state == State.Selected)
            {
                return;
            }

            _state = State.Selected;

            currentSquare.Select();
            Debug.Log($"Piece: {name} is selected.");
            _renderer.material.color = Color.red;
        }

        public void DisableSelect()
        {
            if (_state != State.Selected)
            {
                return;
            }

            _state = State.None;

            currentSquare.DisableSelect();
            Debug.Log($"Piece: {name} selection is disabled.");
            _renderer.material.color = Color.white;
        }

        public void Highlight()
        {
            if (_state == State.Selected || _state == State.Highlighted)
            {
                return;
            }

            _state = State.Highlighted;
            currentSquare?.Highlight();

            Debug.Log($"Piece: {name} is highlighted.");
            _renderer.material.color = Color.blue;
        }

        public void DisableHighlight()
        {
            if (_state != State.Highlighted)
            {
                return;
            }

            _state = State.None;
            currentSquare?.DisableHighlight();

            Debug.Log($"Piece: {name} highlight is disabled.");
            _renderer.material.color = Color.white;
        }

        public void MoveToAndEat(Vector3 position, ISelectable selectable)
        {
            if (_state != State.Selected)
            {
                return;
            }

            if (!gameManager.IsRightTurn(pieceColor))
            {
                return;
            }

            Square square = selectable.GetSquare();
            Piece piece = selectable.GetPiece();

            // Must first check if you can eat
            if (CanEatAt(square))
            {
                piece?.MoveToBeaten();

                DisableSelect();
                Move();
                ResetCurrentSquare(square);
                gameManager.ChangeTurn();
            }
            else if (CanMoveTo(square))
            {
                DisableSelect();
                Move();
                ResetCurrentSquare(square);
                gameManager.ChangeTurn();
            }

            return;

            void Move(TweenCallback callback = null)
            {
                // Move
                float distance = Vector3.Distance(transform.position, position);
                float duration = distance / speed;
                var tween = transform.DOMove(position, duration).SetEase(ease);

                if(callback != null)
                {
                    tween.onComplete = callback;
                }
            }

            void ResetCurrentSquare(Square newSquare)
            {
                currentSquare.SetPiece(null);
                currentSquare = newSquare;
                currentSquare.SetPiece(this);
            }
        }

        protected abstract bool CanEatAt(Square square);

        protected abstract bool CanMoveTo(Square square);

        private void MoveToBeaten()
        {
            transform.position = new Vector3(-1.5f, 0f, 0f);
        }

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

        [ContextMenu("Get Section And Align")]
        private void GetSectionAndAlign()
        {
            Construct(FindObjectOfType<GameManager>());

            if (!Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hitInfo, 2f, squareLayer))
            {
                return;
            }

            transform.position = hitInfo.transform.position;
            currentSquare = hitInfo.collider.GetComponent<Square>();
            currentSquare.SetPiece(this);
        }
    }
}
