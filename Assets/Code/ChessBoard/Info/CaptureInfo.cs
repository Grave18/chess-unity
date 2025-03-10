using AlgebraicNotation;
using ChessBoard.Pieces;

namespace ChessBoard.Info
{
    public struct CaptureInfo
    {
        public readonly Piece BeatenPiece;
        public readonly NotationTurnType NotationTurnType;

        public CaptureInfo(Piece beatenPiece, NotationTurnType notationTurnType = NotationTurnType.Capture)
        {
            BeatenPiece = beatenPiece;
            NotationTurnType = notationTurnType;
        }
    }
}