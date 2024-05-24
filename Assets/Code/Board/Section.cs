using Logic;
using UnityEngine;

namespace Board
{
    public class Section : MonoBehaviour, IHighlightable, ISelectable
    {
        private enum State
        {
            None, Selected, Highlighted
        }

        private Piece currentPiece;
        private MeshRenderer _renderer;
        private State _state = State.None;

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        public void Highlight()
        {
            if (_state == State.Selected || _state == State.Highlighted)
            {
                return;
            }

            _state = State.Highlighted;

            Debug.Log($"Section: {name} is highlighted.");
            currentPiece?.Highlight();
            _renderer.material.color = Color.cyan;
        }


        public void DisableHighlight()
        {
            if (_state != State.Highlighted)
            {
                return;
            }

            _state = State.None;

            Debug.Log($"Section: {name} highlight is disabled.");
            currentPiece?.DisableHighlight();
            _renderer.material.color = Color.white;
        }

        public void Select()
        {
            if (_state == State.Selected)
            {
                return;
            }

            _state = State.Selected;

            Debug.Log($"Section: {name} is selected.");
            currentPiece?.Select();
            _renderer.material.color = Color.magenta;
        }

        public void DisableSelect()
        {
            if (_state != State.Selected)
            {
                return;
            }

            _state = State.None;

            Debug.Log($"Section: {name} selection is disabled.");
            currentPiece?.DisableSelect();
            _renderer.material.color = Color.white;
        }

        public void Move(Vector3 target)
        {
            if (_state != State.Selected)
            {
                return;
            }

            if (currentPiece == null)
            {
                return;
            }

            Debug.Log($"Section: {name} delegate move to {currentPiece.name}.");
            currentPiece.Move(target);
        }

        public void SetPiece(Piece piece)
        {
            currentPiece = piece;
        }

        public bool IsEqual(ISelectable other)
        {
            return this == other || currentPiece == other;
        }

        public bool IsEqual(IHighlightable other)
        {
            return this == other || currentPiece == other;
        }
    }
}
