using GameAndScene;
using Network;
using Notation;
using Settings;
using MvvmTool;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public partial class InGamePageViewModel : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private PopupViewModel popupViewModel;
        [SerializeField] private InGameMenuViewModel inGameMenuViewModel;

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
            if(isOpened)
            {
                OpenRematchPopupCommand.NotifyCanExecuteChanged();
                OpenDrawPopupCommand.NotifyCanExecuteChanged();
                OpenResignPopupCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand(CanExecute = nameof(OpenRematchPopup_CanExecute))]
        private void OpenRematchPopup()
        {
            popupViewModel.OpenRematchPopup();
        }

        [RelayCommand(CanExecute = nameof(OpenDrawPopup_CanExecute))]
        private void OpenDrawPopup()
        {
            popupViewModel.OpenDrawPopup();
        }

        [RelayCommand(CanExecute = nameof(OpenResignPopup_CanExecute))]
        private void OpenResignPopup()
        {
            popupViewModel.OpenResignPopup();
        }

        [RelayCommand]
        private void OpenExitPopup()
        {
            popupViewModel.OpenExitPopup();
        }

        private bool OpenRematchPopup_CanExecute()
        {
            return OnlineInstanceHandler.IsOffline || (OnlineInstanceHandler.IsOnline && game.IsGameOver);
        }

        private bool OpenDrawPopup_CanExecute()
        {
            return OnlineInstanceHandler.IsOnline && !game.IsGameOver && game.IsMyTurn;
        }

        private bool OpenResignPopup_CanExecute()
        {
            return !game.IsGameOver && game.IsMyTurn;
        }
    }
}