using UnityEngine;

namespace Chess3D.Runtime.UtilsProject
{
    public class BeatenPiecePlacer : MonoBehaviour
    {
#if UNITY_EDITOR

        [SerializeField] private Transform parent;

        [Space]
        [SerializeField] private int width = 3;
        [SerializeField] private int height = 5;
        [SerializeField] private float spacingX = 0.09f;
        [SerializeField] private float spacingY = 0.09f;


        private void OnValidate()
        {
            Vector3 anchorPos = parent.GetChild(0).position;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pos = new Vector3(anchorPos.x + x * spacingX, anchorPos.y, anchorPos.z + y * spacingY);
                    parent.GetChild(x + y * 3).position = pos;
                }
            }
        }

#endif
    }
}
