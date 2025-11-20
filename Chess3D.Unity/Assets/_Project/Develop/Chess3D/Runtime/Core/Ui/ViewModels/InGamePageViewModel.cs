using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Sound;
using Chess3D.Runtime.Online;
using MvvmTool;
using PurrNet;
using VContainer;

namespace Chess3D.Runtime.Core.Ui.ViewModels
{
    public partial class InGamePageViewModel : NetworkBehaviour
    {
        private Game _game;
        private PopupViewModel _popupViewModel;
        private InGameMenuViewModel _inGameMenuViewModel;
        private EffectsPlayer _effectsPlayer;

        [Inject]
        public void Construct(Game game, PopupViewModel popupViewModel, InGameMenuViewModel inGameMenuViewModel, EffectsPlayer effectsPlayer)
        {
            _game = game;
            _popupViewModel = popupViewModel;
            _inGameMenuViewModel = inGameMenuViewModel;
            _effectsPlayer = effectsPlayer;
        }

        public bool IsRematchRequested { get; set; }
        public bool IsDrawRequested { get; set; }

        private void OnEnable()
        {
            _inGameMenuViewModel.OnOpenedChanged += OnInGameMenuOpenedChanged;
        }

        private void OnDisable()
        {
            _inGameMenuViewModel.OnOpenedChanged -= OnInGameMenuOpenedChanged;
        }

        private void OnInGameMenuOpenedChanged(bool isOpened)
        {
            OpenRematchPopupCommand.NotifyCanExecuteChanged();
            OpenDrawPopupCommand.NotifyCanExecuteChanged();
            OpenResignPopupCommand.NotifyCanExecuteChanged();
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

        private bool OpenRematchPopup_CanExecute()
        {
            bool isOffline = OnlineInstanceHandler.IsOffline;
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested && _game.IsEndGame;
            bool canOpen = isOffline || isOnline;
            return canOpen;
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

        private bool OpenDrawPopup_CanExecute()
        {
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested && !_game.IsEndGame && _game.IsMyTurn;
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

        private bool OpenResignPopup_CanExecute()
        {
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested&& !_game.IsEndGame && _game.IsMyTurn;
            return isOnline;
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