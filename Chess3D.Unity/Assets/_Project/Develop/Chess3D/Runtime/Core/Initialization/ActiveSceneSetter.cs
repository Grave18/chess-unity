using Chess3D.Runtime.Utilities.Common.SceneTool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chess3D.Runtime.Core.Initialization
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