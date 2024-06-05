using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameAndScene
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        public void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
