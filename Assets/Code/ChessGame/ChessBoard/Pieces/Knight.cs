using ChessGame.ChessBoard.Info;
using UnityEngine;

namespace ChessGame.ChessBoard.Pieces
{
    public class Knight : Piece
    {
        [Header("Knight")]
        [SerializeField] private Vector2Int[] moves;

        protected override void CalculateMovesAndCapturesInternal()
        {
            foreach (Vector2Int offset in moves)
            {
                Square square = Game.GetSquareRel(pieceColor, currentSquare, offset);

                if (square == Game.NullSquare)
                {
                    continue;
                }

                if (!square.HasPiece())
                {
                    MoveSquares.Add(square, new MoveInfo());
                }
                else if (square.GetPieceColor() != pieceColor)
                {
                    CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
                }
                else if (square.GetPieceColor() == pieceColor)
                {
                    DefendSquares.Add(square);
                }
            }
        }
    }
}