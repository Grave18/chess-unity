using UnityEngine;
using UnityEngine.Serialization;

namespace Board.Pieces
{
    public class Pawn : Piece
    {
        [Header("Pawn")]
        [FormerlySerializedAs("MovesFirstMove")] [SerializeField] private Vector2Int[] movesFirstMove;
        [FormerlySerializedAs("Moves")] [SerializeField] private Vector2Int[] moves;
        [FormerlySerializedAs("Eat")] [SerializeField] private Vector2Int[] eat;

        protected override void CalculateMovesAndCapturesInternal()
        {
            Vector2Int[] currentMoves = IsFirstMove ? movesFirstMove : moves;
            foreach (Vector2Int offset in currentMoves)
            {
                Square square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                if (square.HasPiece() || square == gameManager.NullSquare)
                {
                    break;
                }

                MoveSquares.Add(square);
            }

            // Calculate Captures
            foreach (Vector2Int offset in eat)
            {
                Square square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                if (square.HasPiece())
                {
                    if (square.GetPieceColor() != pieceColor)
                    {
                        CaptureSquares.Add(square);
                    }
                    else
                    {
                        DefendSquares.Add(square);
                    }
                }
            }
        }
    }
}