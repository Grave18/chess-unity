using Chess3D.Runtime.ChessBoard.Info;
using Chess3D.Runtime.Logic;

namespace Chess3D.Runtime.Notation
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