using Noesis;
using UnityEngine;
using Image = Noesis.Image;

namespace Ui.Noesis
{
    public class UiRenderTextureSetter : MonoBehaviour
    {
        [SerializeField] private RenderTexture renderTexture;

        private void Start()
        {
            PlugInTextureToUi();
        }

        private void PlugInTextureToUi()
        {
            var noesisView = GetComponent<NoesisView>();
            var userControl = noesisView.Content as UserControl;

            Image image = null;
            if (userControl != null)
            {
                image = userControl.FindName("image") as Image;
            }

            TextureSource textureSource = null;
            if(renderTexture)
            {
                textureSource = new TextureSource(renderTexture);
            }

            if (image != null)
            {
                image.Source = textureSource;
            }
        }
    }
}