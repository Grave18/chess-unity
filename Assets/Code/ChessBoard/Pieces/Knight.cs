using UnityEngine;
using UnityEngine.Serialization;

namespace ChessBoard.Pieces
{
    public class Knight : Piece
    {
        [Header("Knight")]
        [FormerlySerializedAs("Moves")]
        [SerializeField] private Vector2Int[] moves;

        protected override void CalculateMovesAndCapturesInternal()
        {
            foreach (Vector2Int offset in moves)
            {
                Square square = game.GetSquareRel(pieceColor, currentSquare, offset);

                if (square == game.NullSquare)
                {
                    continue;
                }

                if (!square.HasPiece())
                {
                    MoveSquares.Add(square);
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