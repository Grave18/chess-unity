using UnityEngine;

namespace GameLifeManagement
{
    public class ExitGame : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}