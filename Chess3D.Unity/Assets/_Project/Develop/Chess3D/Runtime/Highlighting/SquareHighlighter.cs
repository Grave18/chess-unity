using UnityEngine;

namespace Chess3D.Runtime.Highlighting
{
    public class SquareHighlighter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite dot;
        [SerializeField] private Sprite circle;
        [SerializeField] private Sprite cross;

        public void Show(SquareShape shape, Color color)
        {
            spriteRenderer.sprite = shape switch
            {
                SquareShape.Dot => dot,
                SquareShape.Circle => circle,
                SquareShape.Cross => cross,
                SquareShape.None => null,
                _ => throw new System.ArgumentException("Unexpected SquareShape")
            };

            spriteRenderer.color = color;
        }
    }
}
