#pragma warning disable CS0252, CS0253

using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic;
using UnityEngine;

namespace Chess3D.Runtime.Core.ChessBoard
{
    public class Square : MonoBehaviour, ISelectable
    {
        [field:SerializeField] public int X { get; set; }
        [field:SerializeField] public int Y { get; set; }
        /// File = a, b, c, d, e, f, g, h
        [field:SerializeField] public string File { get; set; }
        /// Rank = 1, 2, 3, 4, 5, 6, 7, 8
        [field:SerializeField] public string Rank { get; set; }
        /// Address = a1, b1, c1, d1, e1, f1, g1, h1, a2 ...
        [field:SerializeField] public string Address { get; set; }

        [Header("Debug")]
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
