using UnityEngine;

namespace Board
{
    public class Pawn : Piece
    {
        [Header("Pawn")]
        public Vector2Int[] MovesFirstMove;
        public Vector2Int[] Moves;
        public Vector2Int[] Eat;
        [SerializeField] private bool isFirstMove = true;

        protected override bool CanEatAt(Section section)
        {
            // Early exit if section has no piece or piece of the same color
            if (!section.HasPiece() || section.GetPieceColor() == pieceColor)
            {
                return false;
            }

            foreach (Vector2Int offset in Eat)
            {
                var possibleSection = gameManager.GetSection(currentSection, offset);
                Debug.Log($"Can eat: {possibleSection.name}");

                if (possibleSection == section)
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool CanMoveTo(Section section)
        {
            if (section.HasPiece())
            {
                return false;
            }

            Vector2Int[] move;
            if (isFirstMove)
            {
                move = MovesFirstMove;

                // If move two sections check if passing section is empty
                var passedSection = gameManager.GetSection(currentSection, move[0]);
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
                var possibleSection = gameManager.GetSection(currentSection, offset);
                Debug.Log($"Can Move: {possibleSection.name}");

                if (possibleSection == section)
                {
                    isFirstMove = false;

                    return true;
                }
            }

            return false;
        }
    }
}