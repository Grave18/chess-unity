using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScenesManagement
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
