using System.Collections;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PlayTests
{
    public class RunGameTest
    {
        [UnityTest]
        public IEnumerator RunGameTestWithEnumeratorPasses()
        {
            var gameSettingsContainerObj = new GameObject("GameSettingsContainer");
            var gameSettingsContainer = gameSettingsContainerObj.AddComponent<GameSettingsContainer>();
            gameSettingsContainer.Init();
            gameSettingsContainer.SetupGameOffline();

            yield return SceneManager.LoadSceneAsync("GameScene");

            yield return new WaitForSecondsRealtime(5f);

            yield return SceneManager.UnloadSceneAsync("GameScene");
        }
    }
}
