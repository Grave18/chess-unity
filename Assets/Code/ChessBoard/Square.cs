#pragma warning disable CS0252, CS0253

using ChessBoard.Pieces;
using Logic;
using UnityEngine;

namespace ChessBoard
{
    public class Square : MonoBehaviour, ISelectable
    {
        [field:SerializeField] public int X { get; set; }
        [field:SerializeField] public int Y { get; set; }
        [field:SerializeField] public string File { get; set; }
        [field:SerializeField] public string Rank { get; set; }

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
