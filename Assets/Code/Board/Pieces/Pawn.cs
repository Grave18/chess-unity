using UnityEngine;

namespace Board.Pieces
{
    public class Pawn : Piece
    {
        [Header("Pawn")]
        public Vector2Int[] MovesFirstMove;
        public Vector2Int[] Moves;
        public Vector2Int[] Eat;
        [SerializeField] private bool isFirstMove = true;

        protected override bool CanEatAt(Square square)
        {
            // Early exit if section has no piece or piece of the same color
            if (!square.HasPiece() || square.GetPieceColor() == pieceColor)
            {
                return false;
            }

            foreach (Vector2Int offset in Eat)
            {
                var possibleSquare = gameManager.GetSquare(currentSquare, offset);
                Debug.Log($"Can eat: {possibleSquare.name}");

                if (possibleSquare == square)
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool CanMoveTo(Square square)
        {
            if (square.HasPiece())
            {
                return false;
            }

            Vector2Int[] move;
            if (isFirstMove)
            {
                move = MovesFirstMove;

                // If move two sections check if passing section is empty
                var passedSection = gameManager.GetSquare(currentSquare, move[0]);
                if (passedSection.HasPiece())
                {
                    return false;
                }
            }
            else
            {
                move = Moves;
            }

            foreach (Vector2Int offset in move)
            {
                var possibleSection = gameManager.GetSquare(currentSquare, offset);
                Debug.Log($"Can Move: {possibleSection.name}");

                if (possibleSection == square)
                {
                    isFirstMove = false;

                    return true;
                }
            }

            return false;
        }
    }
}