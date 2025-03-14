using UnityEngine;

namespace Utils
{
    public class DebugGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.red;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, 0.03f);
        }

#endif
    }
}