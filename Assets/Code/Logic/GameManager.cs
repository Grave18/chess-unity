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

        public void ChangeMove(int index)
        {
            CurrentTurn = (Turn)index;
        }

        public Section GetSection(Section currentSection, int offsetX, int offsetY)
        {
            int x;
            int y;

            if (CurrentTurn == Turn.White)
            {
                x = currentSection.X + offsetX;
                y = currentSection.Y + offsetY;
            }
            else
            {
                x = currentSection.X - offsetX;
                y = currentSection.Y - offsetY;
            }

            // if out of board bounds
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                // Return Null section (last in array)
                return Sections[^1];
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
