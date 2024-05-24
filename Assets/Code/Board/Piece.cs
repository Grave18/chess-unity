using DG.Tweening;
using Logic;
using UnityEngine;

namespace Board
{
    public class Piece : MonoBehaviour, IHighlightable, ISelectable
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

            currentSection?.Highlight();
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

            currentSection?.DisableHighlight();
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

            currentSection?.Select();
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

            currentSection?.DisableSelect();
            Debug.Log($"Piece: {name} selection is disabled.");
            _renderer.material.color = Color.white;
        }

        public void Move(Vector3 target)
        {
            if (_state != State.Selected)
            {
                return;
            }

            var tween = transform.DOMove(target, 2).SetEase(Ease.InOutCubic);
            tween.onComplete = () => currentSection.SetPiece(this);
        }

        public bool IsEqual(ISelectable other)
        {
            return this == other || currentSection == other;
        }

        public bool IsEqual(IHighlightable other)
        {
            return this == other || currentSection == other;
        }
    }
}
