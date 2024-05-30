using System.Collections.Generic;
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

        [SerializeField] private CommonPieceSettings commonSettings;
        [SerializeField] private List<Square> underAttackSquares;

        private MeshRenderer _renderer;
        private State _state = State.None;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        // Getters
        public List<Square> UnderAttackSquares => underAttackSquares;

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
            _renderer.material.EnableKeyword("_EMISSION");
        }

        public void SetCommonPieceSettings(CommonPieceSettings commonSettings)
        {
            this.commonSettings = commonSettings;
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

            // _renderer.material.color = Color.red;
            _renderer.material.SetColor(EmissionColor, commonSettings.SelectColor);
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
            // _renderer.material.color = Color.white;
            _renderer.material.SetColor(EmissionColor, Color.black);
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
            // _renderer.material.color = Color.blue;
            _renderer.material.SetColor(EmissionColor, commonSettings.HighLightColor);
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
            // _renderer.material.color = Color.white;
            _renderer.material.SetColor(EmissionColor, Color.black);
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
                float duration = distance / commonSettings.Speed;
                var tween = transform.DOMove(position, duration).SetEase(commonSettings.Ease);

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
        public abstract void CalculateUnderAttackSquares();

        private void MoveToBeaten()
        {
            transform.position = new Vector3(-1.5f, 0f, 0f);
            // currentSquare = null;
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

            if (!Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hitInfo, 2f, commonSettings.SquareLayer))
            {
                return;
            }

            transform.position = hitInfo.transform.position;
            currentSquare = hitInfo.collider.GetComponent<Square>();
            currentSquare.SetPiece(this);
        }
    }
}
