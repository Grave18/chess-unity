using System;
using System.Linq;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Sound;
using Chess3D.Runtime.Menu.UI;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Online;
using Cysharp.Threading.Tasks;
using MvvmTool;
using PurrNet;
using VContainer;
using UniRx;

namespace Chess3D.Runtime.Core.UI.ViewModels
{
    // TODO: Refactor NetworkBehaviour out of this class
    public sealed partial class CoreHomeViewModel : NetworkBehaviour, IViewModel, IDisposable
    {
        public bool IsRematchRequested { get; set; }
        public bool IsDrawRequested { get; set; }

        private IObjectResolver _resolver;
        private Game _game;
        private PopupViewModel _popupViewModel;
        private EffectsPlayer _effectsPlayer;
        private ViewNavigator _viewNavigator;
        private ConfigContainer _configContainer;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(IObjectResolver resolver, Game game, PopupViewModel popupViewModel, CoreMainViewModel coreMainViewModel,
            EffectsPlayer effectsPlayer, ViewNavigator viewNavigator, ConfigContainer configContainer)
        {
            _resolver = resolver;
            _game = game;
            _popupViewModel = popupViewModel;
            _effectsPlayer = effectsPlayer;
            _viewNavigator = viewNavigator;
            _configContainer = configContainer;

            coreMainViewModel.OnOpenedChanged
                .Subscribe(UpdateButtonState)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void UpdateButtonState(bool isOpened)
        {
            if (!isOpened)
            {
                return;
            }

            OpenRematchPopupCommand.NotifyCanExecuteChanged();
            OpenDrawPopupCommand.NotifyCanExecuteChanged();
            OpenResignPopupCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void Settings()
        {
            _viewNavigator.GoTo<SettingsViewModel>();
        }

        private bool OpenRematchPopup_CanExecute()
        {
            bool isOffline = _configContainer.OnlineConfig.ConnectionType == ConnectionType.Offline;
            bool isOnline = _configContainer.OnlineConfig.ConnectionType == ConnectionType.Online && !IsRematchRequested && !IsDrawRequested && _game.IsEndGame;
            bool canOpen = isOffline || isOnline;
            return canOpen;
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
            bool isOnline = _configContainer.OnlineConfig.ConnectionType == ConnectionType.Online && !IsRematchRequested && !IsDrawRequested && !_game.IsEndGame && _game.IsMyTurn;
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
            bool isOnline = _configContainer.OnlineConfig.ConnectionType == ConnectionType.Online && !IsRematchRequested && !IsDrawRequested&& !_game.IsEndGame && _game.IsMyTurn;
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
            if (_configContainer.OnlineConfig.ConnectionType == ConnectionType.Offline)
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
            var coreFlow = _resolver.Resolve<CoreFlow>();
            coreFlow?.StartAsync().Forget();
        }

        private void RematchOnline()
        {
            IsRematchRequested = true;
            PlayerID otherPlayerID = GetOtherPlayerID();
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
            RematchOffline();
            _popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = GetOtherPlayerID();
            RematchApprove_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void RematchApprove_TargetRpc(PlayerID player)
        {
            IsRematchRequested = false;
            RematchOffline();
            _popupViewModel.ClosePopupToGame();
        }

        private void RematchDecline()
        {
            PlayerID otherPlayerID = GetOtherPlayerID();
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

            PlayerID otherPlayerID = GetOtherPlayerID();
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

            PlayerID otherPlayerID = GetOtherPlayerID();
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

            PlayerID otherPlayerID = GetOtherPlayerID();
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

            PlayerID otherPlayerID = GetOtherPlayerID();
            Resign_TargetRpc(otherPlayerID);
        }

        private PlayerID GetOtherPlayerID()
        {
            PlayerID? thisPlayerID = localPlayer;
            PlayerID? otherPlayerID = networkManager.players.FirstOrDefault(otherPlayerID => otherPlayerID != thisPlayerID);
            return otherPlayerID.Value;
        }

        [TargetRpc]
        private void Resign_TargetRpc(PlayerID otherPlayerID)
        {
            _game.ResignSetup();
        }
    }
}