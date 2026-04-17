using Chess3D.Runtime.Utilities;
using System.Threading;
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

        public async UniTask Load(IViewModel viewModel, CancellationToken cancellationToken = default)
        {
            LoadStartingView(viewModel);
            await InitRenderTexture(cancellationToken);
        }

        private void LoadStartingView(IViewModel viewModel)
        {
            CurrentViewModel = viewModel;
        }

        private async UniTask InitRenderTexture(CancellationToken cancellationToken)
        {
            if(_renderTexture)
            {
                await UniTask.NextFrame(cancellationToken);

                ImageSource = new TextureSource(_renderTexture);
            }
        }
    }
}