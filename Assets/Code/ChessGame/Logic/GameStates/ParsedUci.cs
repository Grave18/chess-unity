using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Info;

namespace ChessGame.Logic.GameStates
{
    public struct ParsedUci
    {
        public Square FromSquare;
        public Square ToSquare;
        public PieceType PromotedPieceType;
    }
}