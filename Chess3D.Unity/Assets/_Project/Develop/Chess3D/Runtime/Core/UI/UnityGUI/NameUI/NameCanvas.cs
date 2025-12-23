using Chess3D.Runtime.Core.MainCamera;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.UI.UnityGUI.NameUI
{
    public class NameCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        [Inject]
        public void Construct([Key(CameraKeys.Ui)] Camera uiCamera)
        {
            canvas.worldCamera = uiCamera;
        }
    }
}
