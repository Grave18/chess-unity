using UnityEngine;
using UnityEngine.Serialization;

namespace Board.Pieces
{
    public class Pawn : Piece
    {
        [Header("Pawn")]
        [FormerlySerializedAs("MovesFirstMove")]
        [SerializeField] private Vector2Int[] movesFirstMove;
        [FormerlySerializedAs("Moves")]
        [SerializeField] private Vector2Int[] moves;
        [FormerlySerializedAs("Eat")]
        [SerializeField] private Vector2Int[] eat;

        protected override void CalculateMovesAndCapturesInternal()
        {
            Vector2Int[] currentMoves = IsFirstMove ? movesFirstMove : moves;

            // Calculate moves
            foreach (Vector2Int offset in currentMoves)
            {
                Square square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                if (square.HasPiece() || square == gameManager.NullSquare)
                {
                    break;
                }

                MoveSquares.Add(square);
            }

            // Calculate Captures and defends
            foreach (Vector2Int offset in eat)
            {
                Square square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                // Captures and defends
                if (square.HasPiece())
                {
                    if (square.GetPieceColor() != pieceColor)
                    {
                        CaptureSquares.Add(square, square.GetPiece());
                    }
                    else
                    {
                        DefendSquares.Add(square);
                    }
                }
                // En Passant
                else
                {
                    Square squareWithPawn = gameManager.GetSquareRel(pieceColor, square, new Vector2Int(0, -1));

                    // Check if pawn make 2 squares and moved list turn
                    if(squareWithPawn.HasPiece() && squareWithPawn.GetPiece() is Pawn pawn
                       && gameManager.GetLastMovedPiece() is Pawn lastMovedPawn && lastMovedPawn == pawn)
                    {
                        CaptureSquares.Add(square, pawn);
                    }
                }
            }
        }
    }
}