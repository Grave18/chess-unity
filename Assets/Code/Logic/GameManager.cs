using Board;
using UnityEngine;

namespace Logic
{
    public class GameManager : MonoBehaviour
    {
        public enum Turn
        {
            White, Black
        }

        private const int Width = 8;
        private const int Height = 8;

        public Turn CurrentTurn = Turn.White;
        public Section[] Sections;

        public void Construct(Section[] sections)
        {
            Sections = sections;
        }

        public Section GetSection(int x, int y)
        {
            // If out of bounds return first section
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return Sections[0];
            }

            Section section = CurrentTurn == Turn.White
                ? Sections[y + x * Width]
                // Inverse board indices for black turn
                : Sections[Height - 1 - y + (Width - 1 - x) * Width];

            return section;
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