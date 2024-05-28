using Board;
using UnityEngine;

namespace Logic
{

    public class GameManager : MonoBehaviour
    {

        private const int Width = 8;
        private const int Height = 8;

        public PieceColor CurrentTurn = PieceColor.White;
        public Section[] Sections;

        public Section NullSection => Sections[^1];

        public void Construct(Section[] sections)
        {
            Sections = sections;
        }

        public void ChangeTurn(int index)
        {
            if (index < 0 || index > 1)
            {
                return;
            }

            CurrentTurn = (PieceColor)index;
        }

        public void ChangeTurn(PieceColor turn)
        {
            if (turn == PieceColor.None)
            {
                return;
            }

            CurrentTurn = turn;
        }

        public Section GetSection(Section currentSection, Vector2Int offset)
        {
            int x;
            int y;

            if (CurrentTurn == PieceColor.White)
            {
                x = currentSection.X + offset.x;
                y = currentSection.Y + offset.y;
            }
            else
            {
                x = currentSection.X - offset.x;
                y = currentSection.Y - offset.y;
            }

            // if out of board bounds
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                // Return Null section (last in array)
                return NullSection;
            }

            return Sections[y + x * Width];
        }

        [ContextMenu("Find All Sections")]
        private void FindAllSections()
        {
            Construct(FindObjectsOfType<Section>());

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = y + x * Width;

                    var section = Sections[index];
                    section.X = x;
                    section.Y = y;
                }
            }
        }
    }
}
