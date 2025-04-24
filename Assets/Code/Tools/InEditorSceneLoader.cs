#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tools
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


        private static void LoadScene(string scene)
        {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene($"Assets/Assets/Scenes/{scene}.unity");
        }
    }
}

#endif