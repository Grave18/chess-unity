using ChessGame.ChessBoard.Pieces;
using ChessGame.Logic.MovesBuffer;

namespace ChessGame.ChessBoard.Info
{
    public class CaptureInfo
    {
        public readonly Piece BeatenPiece;
        public readonly MoveType MoveType;

        public CaptureInfo(Piece beatenPiece, MoveType moveType = MoveType.Capture)
        {
            BeatenPiece = beatenPiece;
            MoveType = moveType;
        }
    }
}