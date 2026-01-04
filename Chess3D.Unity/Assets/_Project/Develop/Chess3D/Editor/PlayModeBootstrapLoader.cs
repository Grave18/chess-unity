using UnityEditor;
using UnityEngine.SceneManagement;

namespace Chess3D.Editor
{
    [InitializeOnLoad]
    public static class PlayModeBootstrapLoader
    {
        private static string _previousSceneName;

        static PlayModeBootstrapLoader()
        {
            EditorApplication.playModeStateChanged += Change;
            _previousSceneName = null;
        }

        private static void Change(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                _previousSceneName = SceneManager.GetActiveScene().name;
                SceneLoader.LoadBootstrap();
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                SceneLoader.LoadScene(_previousSceneName);
            }
        }
    }
}