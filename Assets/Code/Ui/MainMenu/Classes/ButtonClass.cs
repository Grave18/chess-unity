using TMPro;
using UnityEngine;

namespace Ui.MainMenu.Classes
{
    [CreateAssetMenu(fileName = "ButtonClass", menuName = "Classes/ButtonClass")]
    public class ButtonClass : ScriptableObject
    {
        public int TextSize = 30;
        public int Width = 60;
        public int Height = 300;
        public TMP_FontAsset Font;
        public Color FgColor = Color.white;
        public Color BgColor = Color.gray;
        public Color BgHighlightColor = Color.yellow;
        public Sprite BackgroundImage;
        public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlignment = VerticalAlignment.Center;
    }
}