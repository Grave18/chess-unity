using UnityEngine;

namespace Chess3D.Runtime.Menu
{
    public class BuildVersionDisplayer : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;
            GUI.skin.label.fontSize = (int)(h * 0.02f);

            string buildVer = "ver: " + Application.version;
            GUI.Label(new Rect(w * 0.94f, h * 0.95f, h * 0.3f, h * 0.1f), buildVer);
        }
    }
}