using DG.Tweening;
using UnityEngine;

namespace Board
{
    public class Piece : MonoBehaviour
    {
        private enum State
        {
            None, Selected, Highlighted
        }

        [SerializeField] private Section currentSection;

        private MeshRenderer _renderer;
        private State _state = State.None;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            currentSection.SetPiece(this);
        }

        public void Highlight()
        {
            if (_state is State.Selected or State.Highlighted)
            {
                return;
            }

            _state = State.Highlighted;

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

            Debug.Log($"Piece: {name} highlight is disabled.");
            _renderer.material.color = Color.white;
        }

        public void Select()
        {
            if (_state == State.Selected)
            {
                return;
            }

            _state = State.Selected;

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

            Debug.Log($"Piece: {name} selection is disabled.");
            _renderer.material.color = Color.white;
        }

        public void Move(Vector3 position, Section section)
        {
            if (_state != State.Selected)
            {
                return;
            }

            var tween = transform.DOMove(position, duration: 1).SetEase(Ease.InOutCubic);
            tween.onComplete = () =>
            {
                // Can't be null
                currentSection.SetPiece(null);
                currentSection = section;
                // Can be null
                currentSection?.SetPiece(this);
            };
        }
    }
}
