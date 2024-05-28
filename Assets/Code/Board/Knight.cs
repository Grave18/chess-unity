using UnityEngine;

namespace Board
{
    public class Knight : Piece
    {
        [Header("Knight")]
        public Vector2Int[] Moves;

        protected override bool CanEatAt(Section section)
        {
            return CanMoveTo(section);
        }

        protected override bool CanMoveTo(Section section)
        {
            if (section.HasPiece() && section.GetPieceColor() == pieceColor)
            {
                return false;
            }

            foreach (Vector2Int offset in Moves)
            {
                var possibleSection = gameManager.GetSection(currentSection, offset);
                Debug.Log(possibleSection.name);

                if (possibleSection == section)
                {
                    return true;
                }
            }

            return false;
        }
    }
}