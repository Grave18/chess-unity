using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UtilsCommon.SceneTool;

public class SetSceneActive : MonoBehaviour
{
    [FormerlySerializedAs("sceneReference")]
    [SerializeField] private SceneReference gameScene;

    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(gameScene.ScenePath));
    }
}