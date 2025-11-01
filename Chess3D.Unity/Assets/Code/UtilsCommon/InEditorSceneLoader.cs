#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

namespace UtilsCommon
{
    public static class InEditorSceneLoader
    {
        [MenuItem("Tools/Load Main Menu &1")]
        private static void LoadMainMenu()
        {
            LoadScene("MainMenuScene");
        }

        [MenuItem("Tools/Load Game &2")]
        private static void LoadGame()
        {
            LoadScene("GameScene");
        }

        [MenuItem("Tools/Load Online &3")]
        private static void LoadOnline()
        {
            LoadScene("OnlineScene");
        }

        [MenuItem("Tools/Load Loading &4")]
        private static void LoadLoading()
        {
            LoadScene("LoadingScene");
        }

        private static void LoadScene(string scene)
        {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene($"Assets/Assets/Scenes/{scene}.unity");
        }
    }
}

#endif