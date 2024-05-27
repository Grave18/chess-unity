using UnityEngine;

namespace Board
{
    public class Knight : Piece
    {
        [Header("Knight")]
        public Vector2Int[] Moves;

        protected override bool CanMovePiece(Section section)
        {
            foreach (Vector2Int vector in Moves)
            {
                var possibleSection = gameManager.GetSection(currentSection.X + vector.x, currentSection.Y + vector.y);
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