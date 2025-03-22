using ChessBoard;
using ChessBoard.Info;

namespace Logic.Players.GameStates
{
    public struct ParsedUci
    {
        public Square FromSquare;
        public Square ToSquare;
        public PieceType PromotedPieceType;
    }
}