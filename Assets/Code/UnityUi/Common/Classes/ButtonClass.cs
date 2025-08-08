using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityUi.Common.Classes
{
    [CreateAssetMenu(fileName = "ButtonClass", menuName = "Classes/ButtonClass")]
    public class ButtonClass : ScriptableObject
    {
        [Header("Text")]
        public int TextSize = 30;
        public TMP_FontAsset Font;
        [FormerlySerializedAs("FgColor")] public Color TextColor = Color.white;
        public Color TextSelectedColor = Color.red;
        public Color TextDisabledColor = Color.black;

        [Header("Button")]
        public int Height = 300;
        public int Width = 60;
        public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlignment = VerticalAlignment.Center;

        [Header("Background")]
        public Color BgColor = Color.gray;
        public Color BgHighlightColor = Color.yellow;
        public Sprite BackgroundImage;
    }
}