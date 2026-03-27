using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Sound;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Cysharp.Threading.Tasks;
using MvvmTool;
using PurrNet;
using VContainer;

namespace Chess3D.Runtime.Core.UI.ViewModels
{
    public sealed partial class CoreHomeViewModel : NetworkBehaviour, IViewModel
    {
        public bool IsDrawRequested { get; set; }

        private IObjectResolver _resolver;
        private Game _game;
        private PopupViewModel _popupViewModel;
        private EffectsPlayer _effectsPlayer;
        private ViewNavigator _viewNavigator;

        [Inject]
        public void Construct(IObjectResolver resolver, Game game, PopupViewModel popupViewModel, ViewNavigator viewNavigator)
        {
            _resolver = resolver;
            _game = game;
            _popupViewModel = popupViewModel;
            _viewNavigator = viewNavigator;
        }

        [RelayCommand]
        private void Settings()
        {
            _viewNavigator.GoTo<SettingsViewModel>();
        }

        private bool OpenRematchPopup_CanExecute()
        {
            return true;
        }

        [RelayCommand(CanExecute = nameof(OpenRematchPopup_CanExecute))]
        private void OpenRematchPopup()
        {
            var popupSettings = new PopupViewModel.PopupSettings
            {
                PopupText = "Do you want to Rematch?",
                PopupYesCommand = Rematch,
                PopupNoCommand = _popupViewModel.ClosePopupToPause,
            };

            _popupViewModel.OpenConfigurablePopup(popupSettings);
        }

        private bool OpenDrawPopup_CanExecute()
        {
            return true;
        }

        [RelayCommand(CanExecute = nameof(OpenDrawPopup_CanExecute))]
        private void OpenDrawPopup()
        {
            var popupSettings = new PopupViewModel.PopupSettings
            {
                PopupText = "Do you want to Draw?",
                PopupYesCommand = Draw,
                PopupNoCommand = _popupViewModel.ClosePopupToPause,
            };

            _popupViewModel.OpenConfigurablePopup(popupSettings);
        }

        private bool OpenResignPopup_CanExecute()
        {
            return true;
        }

        [RelayCommand(CanExecute = nameof(OpenResignPopup_CanExecute))]
        private void OpenResignPopup()
        {
            var popupSettings = new PopupViewModel.PopupSettings
            {
                PopupText = "Do you want to Resign?",
                PopupYesCommand = Resign,
                PopupNoCommand = _popupViewModel.ClosePopupToPause,
            };

            _popupViewModel.OpenConfigurablePopup(popupSettings);
        }

        [RelayCommand]
        private void OpenExitPopup()
        {
            _popupViewModel.OpenExitPopup();
        }

        private void Rematch()
        {
            var coreFlow = _resolver.Resolve<CoreFlow>();
            coreFlow?.StartAsync().Forget();

            _popupViewModel.ClosePopupToGame();
        }

        private void Draw()
        {
            IsDrawRequested = true;
            _popupViewModel.ClosePopupToGame();
        }

        private void Resign()
        {
            _game.ResignSetup();
            _popupViewModel.ClosePopupToGame();
        }
    }
}