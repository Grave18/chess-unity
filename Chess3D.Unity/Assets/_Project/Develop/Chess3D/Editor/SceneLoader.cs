using UnityEditor;
using UnityEditor.SceneManagement;

namespace Chess3D.Editor
{
    public static class SceneLoader
    {
        [MenuItem("Chess3D/Scenes/Load Bootstrap &1")]
        private static void LoadBootstrap()
        {
            LoadScene("0.Bootstrap");
        }

        [MenuItem("Chess3D/Scenes/Load Menu &2")]
        private static void LoadMenu()
        {
            LoadScene("2.Menu");
        }

        [MenuItem("Chess3D/Scenes/Load Game &3")]
        private static void LoadCore()
        {
            LoadScene("3.Core");
        }

        [MenuItem("Chess3D/Scenes/Load Online &4")]
        private static void LoadOnline()
        {
            LoadScene("4.Online");
        }

        [MenuItem("Chess3D/Scenes/Load Logo &5")]
        private static void LoadLogo()
        {
            LoadScene("1.Logo");
        }

        [MenuItem("Chess3D/Scenes/Load LoadScreen &6")]
        private static void LoadLoadScreen()
        {
            LoadScene("5.LoadScreen");
        }

        private static void LoadScene(string scene)
        {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{scene}.unity");
        }
    }
}