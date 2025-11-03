using UnityEngine;

namespace Chess3D.Runtime.Utilities.Common
{
    public class GameObjectDestroyer : MonoBehaviour
    {
        [SerializeField] private bool destroyInEditor;
        [SerializeField] private bool destroyInDevelopmentBuild;
        [SerializeField] private bool destroyInReleaseBuild;

        private void Awake()
        {
            DestroyIfCondition();
        }

        private void DestroyIfCondition()
        {
#if UNITY_EDITOR
            if (destroyInEditor)
            {
                Destroy(gameObject);
            }
#elif DEVELOPMENT_BUILD
            if (destroyInDevelopmentBuild)
            {
                Destroy(gameObject);
            }
#else
            if (destroyInReleaseBuild)
            {
                Destroy(gameObject);
            }
#endif
        }
    }
}