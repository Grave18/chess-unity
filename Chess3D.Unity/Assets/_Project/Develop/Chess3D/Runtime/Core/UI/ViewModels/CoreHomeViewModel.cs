using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Sound;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Online;
using MvvmTool;
using PurrNet;
using VContainer;

namespace Chess3D.Runtime.Core.UI.ViewModels
{
    public partial class CoreHomeViewModel : NetworkBehaviour, IViewModel
    {
        public bool IsRematchRequested { get; set; }
        public bool IsDrawRequested { get; set; }

        private Game _game;
        private PopupViewModel _popupViewModel;
        private CoreMainViewModel _coreMainViewModel;
        private EffectsPlayer _effectsPlayer;
        private ViewNavigator _viewNavigator;

        [Inject]
        public void Construct(Game game, PopupViewModel popupViewModel, CoreMainViewModel coreMainViewModel,
            EffectsPlayer effectsPlayer, ViewNavigator viewNavigator)
        {
            _game = game;
            _popupViewModel = popupViewModel;
            _coreMainViewModel = coreMainViewModel;
            _effectsPlayer = effectsPlayer;
            _viewNavigator = viewNavigator;
        }

        private void OnEnable()
        {
            _coreMainViewModel.OnOpenedChanged += OnCoreMainOpenedChanged;
        }

        private void OnDisable()
        {
            _coreMainViewModel.OnOpenedChanged -= OnCoreMainOpenedChanged;
        }

        private void OnCoreMainOpenedChanged(bool isOpened)
        {
            OpenRematchPopupCommand.NotifyCanExecuteChanged();
            OpenDrawPopupCommand.NotifyCanExecuteChanged();
            OpenResignPopupCommand.NotifyCanExecuteChanged();
        }

        private bool OpenRematchPopup_CanExecute()
        {
            bool isOffline = OnlineInstanceHandler.IsOffline;
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested && _game.IsEndGame;
            bool canOpen = isOffline || isOnline;
            return canOpen;
        }

        [RelayCommand]
        private void Settings()
        {
            _viewNavigator.GoTo<SettingsViewModel>();
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
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested && !_game.IsEndGame && _game.IsMyTurn;
            return isOnline;
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
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested&& !_game.IsEndGame && _game.IsMyTurn;
            return isOnline;
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
            if (OnlineInstanceHandler.IsOffline)
            {
                RematchOffline();
            }
            else
            {
                RematchOnline();
            }

            _popupViewModel.ClosePopupToGame();
        }

        private void RematchOffline()
        {
            // TODO: Flow
            // _game.Rematch();
        }

        private void RematchOnline()
        {
            IsRematchRequested = true;
            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            RequestRematchFrom_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void RequestRematchFrom_TargetRpc(PlayerID playerId)
        {
            var popupSettings = new PopupViewModel.PopupSettings
            {
                IsPopupNoButtonEnabled = true,
                IsSaveCheckBoxEnabled = false,
                PopupText = "Rematch Requested! Rematch?",
                PopupYesCommand = RematchApprove,
                PopupNoCommand = RematchDecline,
            };

            _popupViewModel.OpenConfigurablePopup(popupSettings);

            _effectsPlayer.PlayNotifySound();
        }

        private void RematchApprove()
        {
            // TODO: Flow
            // _game.Rematch();
            _popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            RematchApprove_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void RematchApprove_TargetRpc(PlayerID player)
        {
            IsRematchRequested = false;
            // TODO: Flow
            // _game.Rematch();
            _popupViewModel.ClosePopupToGame();
        }

        private void RematchDecline()
        {
            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            RematchDecline_TargetRpc(otherPlayerID);

            _popupViewModel.ClosePopupToGame();
        }

        private void RematchDecline_TargetRpc(PlayerID otherPlayerID)
        {
            IsRematchRequested = false;
            _popupViewModel.ClosePopupToGame();
        }

        private void Draw()
        {
            IsDrawRequested = true;
            _popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            DrawRequest_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void DrawRequest_TargetRpc(PlayerID otherPlayerID)
        {
            var popupSettings = new PopupViewModel.PopupSettings
            {
                IsPopupNoButtonEnabled = true,
                IsSaveCheckBoxEnabled = false,
                PopupText = "Draw by agreement requested! Draw?",
                PopupYesCommand = DrawApprove,
                PopupNoCommand = DrawDecline,
            };

            _popupViewModel.OpenConfigurablePopup(popupSettings);

            _effectsPlayer.PlayNotifySound();
        }

        private void DrawApprove()
        {
            _game.DrawSetup("Draw by agreement");
            _popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            DrawApprove_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void DrawApprove_TargetRpc(PlayerID otherPlayerID)
        {
            IsDrawRequested = false;
            _game.DrawSetup("Draw by agreement");
        }

        private void DrawDecline()
        {
            _popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            DrawDecline_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void DrawDecline_TargetRpc(PlayerID otherPlayerID)
        {
            IsDrawRequested = false;
        }

        private void Resign()
        {
            _game.ResignSetup();
            _popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            Resign_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void Resign_TargetRpc(PlayerID otherPlayerID)
        {
            _game.ResignSetup();
        }
    }
}