using DG.Tweening;
using UnityEngine;

namespace Chess3D.Runtime.ChessBoard
{
    [System.Serializable]
    public class CommonPieceSettings
    {
        [Header("Animation")]
        public LayerMask SquareLayer;
        public Ease Ease = Ease.InOutCubic;
        public float Speed = 0.7f;

        [Header("Highlight Colors")]
        public Color DefaultColor = Color.black;
        public Color SelectColor = Color.red;
        public Color HighLightColor = Color.blue;
        public Color MoveColor = Color.green;
        public Color CaptureColor = Color.red;
        public Color CanNotMoveColor = Color.gray;
        public Color CastlingColor = Color.yellow;
    }
}
