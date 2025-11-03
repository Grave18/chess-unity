using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.Logic;

namespace Chess3D.Runtime.Core.Notation
{
    public struct PieceInfo
    {
        public PieceType PieceType;
        public PieceColor PieceColor;
        public int SquareNum;
        public bool IsFirstMove;

        public PieceInfo(PieceType pieceType, PieceColor pieceColor, int squareNum, bool isFirstMove = false)
        {
            PieceType = pieceType;
            PieceColor = pieceColor;
            IsFirstMove = isFirstMove;
            SquareNum = squareNum;
        }
    }
}