using Chess3D.Runtime.UtilsCommon.SceneTool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chess3D.Runtime.Initialization
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