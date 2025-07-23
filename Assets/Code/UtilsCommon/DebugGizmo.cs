using UnityEngine;

namespace UtilsCommon
{
    public class DebugGizmo : MonoBehaviour
    {
#if UNITY_EDITOR

        [SerializeField] private Color color = Color.red;

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, 0.03f);
        }

#endif
    }
}