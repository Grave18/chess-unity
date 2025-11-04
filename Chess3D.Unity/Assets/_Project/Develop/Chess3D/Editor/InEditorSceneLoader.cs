using UnityEditor;
using UnityEditor.SceneManagement;

namespace Chess3D.Editor
{
    public static class InEditorSceneLoader
    {
        [MenuItem("Tools/Grave/Load Bootstrap &1")]
        private static void LoadBootstrap()
        {
            LoadScene("0.Bootstrap");
        }

        [MenuItem("Tools/Grave/Load Menu &2")]
        private static void LoadMenu()
        {
            LoadScene("2.Menu");
        }

        [MenuItem("Tools/Grave/Load Game &3")]
        private static void LoadCore()
        {
            LoadScene("3.Core");
        }

        [MenuItem("Tools/Grave/Load Online &4")]
        private static void LoadOnline()
        {
            LoadScene("4.Online");
        }

        [MenuItem("Tools/Grave/Load Logo &5")]
        private static void LoadLogo()
        {
            LoadScene("1.Logo");
        }

        [MenuItem("Tools/Grave/Load LoadScreen &6")]
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