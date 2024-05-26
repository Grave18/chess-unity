using UnityEngine;

namespace Board
{
    public class Knight : Piece
    {
        [Header("Pawn")]
        public Vector2Int[] MovesEat;
        public Vector2Int[] MovesFirstMove;
        public Vector2Int[] Moves;
        public bool IsFirstMove = true;

        protected override bool CanMovePiece(Section section)
        {
            Vector2Int[] move = IsFirstMove ? MovesFirstMove : Moves;

            foreach (var vector in move)
            {
                var possibleSection = gameManager.GetSection(currentSection.X + vector.x, currentSection.Y + vector.y);
                Debug.Log(possibleSection.name);

                if (possibleSection == section)
                {
                    IsFirstMove = false;

                    return true;
                }
            }

            return false;
        }
    }
}