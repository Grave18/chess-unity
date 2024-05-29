using DG.Tweening;
using UnityEngine;

namespace Board
{
    [System.Serializable]
    public class CommonPieceSettings
    {
        public LayerMask SquareLayer;
        public Ease Ease = Ease.InOutCubic;
        public float Speed = 0.7f;
        public Color SelectColor = Color.red;
        public Color HighLightColor = Color.blue;
    }
}
