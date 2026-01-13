using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using Noesis;
using UnityEngine;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [INotifyPropertyChanged]
    [UnityEngine.Scripting.Preserve]
    public sealed partial class MainViewModel : IMainViewModel, ILoadUnit<IViewModel>
    {
        [ObservableProperty] private IViewModel _currentViewModel;
        [ObservableProperty] private ImageSource _imageSource;

        private readonly RenderTexture _renderTexture;

        public MainViewModel(RenderTexture renderTexture)
        {
            _renderTexture = renderTexture;
        }

        public async UniTask Load(IViewModel viewModel)
        {
            LoadStartingView(viewModel);
            await InitRenderTexture();
        }

        private void LoadStartingView(IViewModel viewModel)
        {
            CurrentViewModel = viewModel;
        }

        private async UniTask InitRenderTexture()
        {
            if(_renderTexture)
            {
                await UniTask.NextFrame();

                ImageSource = new TextureSource(_renderTexture);
            }
        }
    }
}