using UnityEngine;
using UnityEngine.UI;

namespace GameAndScene
{
    public class ExitGame : MonoBehaviour
    {
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            exitButton?.onClick.AddListener(Quit);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Quit();
            }
        }

        private static void Quit()
        {
            Application.Quit();
        }
    }
}