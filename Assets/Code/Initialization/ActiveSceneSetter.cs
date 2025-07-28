using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsCommon.SceneTool;

namespace Initialization
{
    public class ActiveSceneSetter : MonoBehaviour
    {
        [SerializeField] private SceneReference gameScene;

        private void Start()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(gameScene.ScenePath));
        }
    }
}