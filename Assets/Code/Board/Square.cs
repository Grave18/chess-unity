using Board.Pieces;
using Logic;
using UnityEngine;
#pragma warning disable CS0252, CS0253

namespace Board
{
    public class Square : MonoBehaviour, ISelectable
    {
        public int X;
        public int Y;
        public string File;
        public string Rank;

        [SerializeField] private Piece currentPiece;

        private MeshRenderer _renderer;

        public string AlgebraicName => $"{File}{Rank}";

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

        public Square GetSquare()
        {
            return this;
        }

        public bool IsSquare()
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
