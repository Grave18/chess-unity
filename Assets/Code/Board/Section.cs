using Logic;
using UnityEngine;

namespace Board
{
    public class Section : MonoBehaviour, ISelectable
    {
        public int X;
        public int Y;

        [SerializeField] private Piece currentPiece;

        private MeshRenderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        public void SetPiece(Piece piece)
        {
            currentPiece = piece;
        }

        public Piece GetPiece()
        {
            return currentPiece;
        }

        public void Select()
        {
            _renderer.material.color = Color.magenta;
        }

        public void DisableSelect()
        {
            _renderer.material.color = Color.white;
        }

        public void Highlight()
        {
            _renderer.material.color = Color.cyan;
        }

        public void DisableHighlight()
        {
            _renderer.material.color = Color.white;
        }

        public void MoveToAndEat(Vector3 position, ISelectable selectable)
        {
            // Must be empty
        }

        public PieceColor GetPieceColor()
        {
            return currentPiece?.GetPieceColor() ?? PieceColor.None;
        }

        public Section GetSection()
        {
            return this;
        }

        public bool IsSection()
        {
            return currentPiece == null;
        }

        public bool IsEqual(ISelectable other)
        {
            return this == other || currentPiece == other;
        }

        public bool HasPiece()
        {
            return currentPiece != null;
        }
    }
}
