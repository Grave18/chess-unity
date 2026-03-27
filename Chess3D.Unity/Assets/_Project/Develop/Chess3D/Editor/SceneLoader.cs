using Chess3D.Runtime.Utilities.Logging;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Chess3D.Editor
{
    public static class SceneLoader
    {
        [MenuItem("Chess3D/Scenes/Load Bootstrap &1")]
        public static void LoadBootstrap()
        {
            LoadScene("0.Bootstrap");
        }

        [MenuItem("Chess3D/Scenes/Load Menu &2")]
        public static void LoadMenu()
        {
            LoadScene("2.Menu");
        }

        [MenuItem("Chess3D/Scenes/Load Game &3")]
        public static void LoadCore()
        {
            LoadScene("3.Core");
        }

        [MenuItem("Chess3D/Scenes/Load Online &4")]
        public static void LoadOnline()
        {
            LoadScene("4.Online");
        }

        [MenuItem("Chess3D/Scenes/Load Logo &5")]
        public static void LoadLogo()
        {
            LoadScene("1.Logo");
        }

        [MenuItem("Chess3D/Scenes/Load LoadScreen &6")]
        public static void LoadLoadScreen()
        {
            LoadScene("5.LoadScreen");
        }

        public static void LoadScene([CanBeNull] string sceneName)
        {
            if (sceneName == null)
            {
                Log.Default.D("SceneLoader", "Scene name is null");
                return;
            }

            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{sceneName}.unity");
        }
    }
}