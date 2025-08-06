using GameAndScene;
using Network;
using Notation;
using Settings;
using MvvmTool;
using PurrNet;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public partial class InGamePageViewModel : NetworkBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private PopupViewModel popupViewModel;
        [SerializeField] private InGameMenuViewModel inGameMenuViewModel;
        [SerializeField] private EffectsPlayer effectsPlayer;

        public bool IsRematchRequested { get; set; }
        public bool IsDrawRequested { get; set; }

        private void OnEnable()
        {
            inGameMenuViewModel.OnOpenedChanged += OnInGameMenuOpenedChanged;
        }

        private void OnDisable()
        {
            inGameMenuViewModel.OnOpenedChanged -= OnInGameMenuOpenedChanged;
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
                PopupNoCommand = popupViewModel.ClosePopupToPause,
            };

            popupViewModel.OpenConfigurablePopup(popupSettings);
        }

        private bool OpenRematchPopup_CanExecute()
        {
            bool isOffline = OnlineInstanceHandler.IsOffline;
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested && game.IsGameOver;
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
                PopupNoCommand = popupViewModel.ClosePopupToPause,
            };

            popupViewModel.OpenConfigurablePopup(popupSettings);
        }

        private bool OpenDrawPopup_CanExecute()
        {
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested && !game.IsGameOver && game.IsMyTurn;
            return isOnline;
        }

        [RelayCommand(CanExecute = nameof(OpenResignPopup_CanExecute))]
        private void OpenResignPopup()
        {
            var popupSettings = new PopupViewModel.PopupSettings
            {
                PopupText = "Do you want to Resign?",
                PopupYesCommand = Resign,
                PopupNoCommand = popupViewModel.ClosePopupToPause,
            };

            popupViewModel.OpenConfigurablePopup(popupSettings);
        }

        private bool OpenResignPopup_CanExecute()
        {
            bool isOnline = OnlineInstanceHandler.IsOnline && !IsRematchRequested && !IsDrawRequested&& !game.IsGameOver && game.IsMyTurn;
            return isOnline;
        }

        [RelayCommand]
        private void OpenExitPopup()
        {
            popupViewModel.OpenExitPopup();
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

            popupViewModel.ClosePopupToGame();
        }

        private void RematchOffline()
        {
            game.Rematch();
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

            popupViewModel.OpenConfigurablePopup(popupSettings);

            effectsPlayer.PlayNotifySound();
        }

        private void RematchApprove()
        {
            game.Rematch();
            popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            RematchApprove_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void RematchApprove_TargetRpc(PlayerID player)
        {
            IsRematchRequested = false;
            game.Rematch();
            popupViewModel.ClosePopupToGame();
        }

        private void RematchDecline()
        {
            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            RematchDecline_TargetRpc(otherPlayerID);

            popupViewModel.ClosePopupToGame();
        }

        private void RematchDecline_TargetRpc(PlayerID otherPlayerID)
        {
            IsRematchRequested = false;
            popupViewModel.ClosePopupToGame();
        }

        private void Draw()
        {
            IsDrawRequested = true;
            popupViewModel.ClosePopupToGame();

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

            popupViewModel.OpenConfigurablePopup(popupSettings);

            effectsPlayer.PlayNotifySound();
        }

        private void DrawApprove()
        {
            game.Draw("Draw by agreement");
            popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            DrawApprove_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void DrawApprove_TargetRpc(PlayerID otherPlayerID)
        {
            IsDrawRequested = false;
            game.Draw("Draw by agreement");
        }

        private void DrawDecline()
        {
            popupViewModel.ClosePopupToGame();

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
            game.Resign();
            popupViewModel.ClosePopupToGame();

            PlayerID otherPlayerID = OnlineInstanceHandler.OtherPlayerID;
            Resign_TargetRpc(otherPlayerID);
        }

        [TargetRpc]
        private void Resign_TargetRpc(PlayerID otherPlayerID)
        {
            game.Resign();
        }
    }
}