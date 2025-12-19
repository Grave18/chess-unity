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
        public ImageSource ImageSource { get; private set; }

        private readonly RenderTexture _renderTexture;

        public MainViewModel(RenderTexture renderTexture)
        {
            _renderTexture = renderTexture;
        }

        public UniTask Load(IViewModel viewModel)
        {
            LoadStartingView(viewModel);
            InitRenderTexture();
            return UniTask.CompletedTask;
        }

        private void LoadStartingView(IViewModel viewModel)
        {
            CurrentViewModel = viewModel;
        }

        private void InitRenderTexture()
        {
            if(_renderTexture)
            {
                ImageSource = new TextureSource(_renderTexture);
            }
        }
    }
}