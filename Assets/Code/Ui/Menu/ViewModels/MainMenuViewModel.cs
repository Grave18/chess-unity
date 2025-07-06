using Noesis;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class MainMenuViewModel : MonoBehaviour
    {
        [SerializeField] private RenderTexture renderTexture;

        public ImageSource ImageSource { get; private set; }

        private void Awake()
        {
            InitRenderTexture();
        }

        private void InitRenderTexture()
        {
            if(renderTexture)
            {
                ImageSource = new TextureSource(renderTexture);
            }
        }
    }
}