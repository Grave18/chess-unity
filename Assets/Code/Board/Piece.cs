using DG.Tweening;
using Logic;
using UnityEngine;

namespace Board
{
    public class Piece : MonoBehaviour, ISelectable
    {
        private enum State
        {
            None, Selected, Highlighted
        }

        [SerializeField] private Section currentSection;
        [SerializeField] private LayerMask sectionLayer;
        [SerializeField] private Ease ease = Ease.InOutCubic;
        [SerializeField] private float speed = 0.7f;

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

        public void Select()
        {
            if (_state == State.Selected)
            {
                return;
            }

            _state = State.Selected;

            currentSection.Select();
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

            currentSection.DisableSelect();
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

        public void Move(Vector3 position, ISelectable selectable)
        {
            if (_state != State.Selected)
            {
                return;
            }

            DisableSelect();

            float distance = Vector3.Distance(transform.position, position);
            float duration = distance / speed;
            transform.DOMove(position, duration).SetEase(ease);

            currentSection.SetPiece(null);
            currentSection = selectable as Section;
            currentSection?.SetPiece(this);
        }

        public bool IsEmpty()
        {
            // Muse be always false
            return false;
        }

        public bool IsEqual(ISelectable other)
        {
            return this == other || currentSection == other;
        }


        [ContextMenu("Get Section And Align")]
        private void GetSectionAndAlign()
        {
            if (!Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hitInfo, 2f, sectionLayer))
            {
                return;
            }

            transform.position = hitInfo.transform.position;
            currentSection = hitInfo.collider.GetComponent<Section>();
            currentSection.SetPiece(this);
        }
    }
}
