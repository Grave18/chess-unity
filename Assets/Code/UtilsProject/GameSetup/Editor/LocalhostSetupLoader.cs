using System.Collections;
using ParrelSync;
using Ui.Menu.ViewModels;
using UnityEditor;
using UnityEngine;

namespace UtilsProject.GameSetup
{
    public static class LocalhostSetupLoader
    {
        [MenuItem("Tools/GameSetup/Load Localhost ^#w")]
        public static void LoadLocalhost()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.EnterPlaymode();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Load();
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            }
        }

        private static void Load()
        {
            var playPageViewModel = Object.FindAnyObjectByType<PlayPageViewModel>();
            playPageViewModel?.StartCoroutine(LoadRoutine());

            return;
            IEnumerator LoadRoutine()
            {
                yield return new WaitForSeconds(2f);

                if (ClonesManager.IsClone())
                {
                    playPageViewModel.StartLocalClient(null);
                }
                else
                {
                    playPageViewModel.StartLocalServer(null);
                }
            }
        }
    }
}