using Chess3D.Runtime.ChessBoard.Pieces;
using Chess3D.Runtime.Logic.MovesBuffer;

namespace Chess3D.Runtime.ChessBoard.Info
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