using ChessGame.Logic.MenuStates;
using GameAndScene;
using Notation;
using Settings;
using MvvmTool;
using Ui.Auxiliary;
using UnityEngine;
using UtilsCommon.Singleton;

namespace Ui.InGame.ViewModels
{
    [INotifyPropertyChanged]
    public partial class PopupViewModel : SingletonBehaviour<PopupViewModel>
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private MenuStateMachine menuStateMachine;

        [ObservableProperty] private bool _isPopupOpen;
        [ObservableProperty] private string _popupText;
        [ObservableProperty] private string _popupYesButtonText;
        [ObservableProperty] private string _popupNoButtonText;
        [ObservableProperty] private bool _isPopupNoButtonEnabled;
        [ObservableProperty] private bool _isSaveCheckBoxEnabled;
        [ObservableProperty] private bool _isSaveCheckBoxChecked;

        public DelegateCommand PopupYesCommand { get; set; }
        public DelegateCommand PopupNoCommand { get; set; }

        protected override void Awake()
        {
            base.Awake();
            PopupYesCommand = new DelegateCommand(ClosePopupToPause);
            PopupNoCommand = new DelegateCommand(ClosePopupToPause);
        }

        public void OpenRematchPopup(object obj)
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = true;
            IsSaveCheckBoxEnabled = false;
            PopupText = "Are you want to Rematch?";
            PopupYesButtonText = "Yes";
            PopupNoButtonText = "No";
            PopupYesCommand.ReplaceCommand(Rematch);
            PopupNoCommand.ReplaceCommand(ClosePopupToPause);

            menuStateMachine.OpenPopup();
        }

        public void OpenDrawPopup(object obj)
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = true;
            IsSaveCheckBoxEnabled = false;
            PopupText = "Are you want to Draw?";
            PopupYesButtonText = "Yes";
            PopupNoButtonText = "No";
            PopupYesCommand.ReplaceCommand(Draw);
            PopupNoCommand.ReplaceCommand(ClosePopupToPause);

            menuStateMachine.OpenPopup();
        }

        public void OpenResignPopup(object obj)
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = true;
            IsSaveCheckBoxEnabled = false;
            PopupText = "Are you want to Resign?";
            PopupYesButtonText = "Yes";
            PopupNoButtonText = "No";
            PopupYesCommand.ReplaceCommand(Resign);
            PopupNoCommand.ReplaceCommand(ClosePopupToPause);

            menuStateMachine.OpenPopup();
        }

        public void OpenExitPopup(object obj)
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = true;
            IsSaveCheckBoxEnabled = true;
            PopupText = "Are you want to Exit?";
            PopupYesButtonText = "Yes";
            PopupNoButtonText = "No";
            PopupYesCommand.ReplaceCommand(ExitToMainMenu);
            PopupNoCommand.ReplaceCommand(ClosePopupToPause);

            menuStateMachine.OpenPopup();
        }

        public void OpenReconnectPopup(object o)
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = false;
            IsSaveCheckBoxEnabled = true;
            PopupText = "Wait for other player to reconnect...";
            PopupYesButtonText = "Exit to menu";
            PopupYesCommand.ReplaceCommand(ExitToMainMenu);

            menuStateMachine.OpenPopup();
        }

        public void ClosePopupToPause(object obj = null)
        {
            IsPopupOpen = false;
            menuStateMachine.ClosePopupToPause();
        }

        public void ClosePopupToGame(object obj = null)
        {
            IsPopupOpen = false;
            menuStateMachine.ClosePopupToGame();
        }

        private void Rematch(object obj)
        {
            IsPopupOpen = false;
            game.Rematch();
            menuStateMachine.ClosePopupToGame();
        }

        private void Draw(object obj)
        {
            IsPopupOpen = false;
            game.DrawByAgreement();
            menuStateMachine.ClosePopupToGame();
        }

        private void Resign(object obj)
        {
            IsPopupOpen = false;
            game.Resign();
            menuStateMachine.ClosePopupToGame();
        }

        private void ExitToMainMenu(object obj)
        {
            SaveBoard();
            sceneLoader.LoadMainMenu();
        }

        private void SaveBoard()
        {
            if (IsSaveCheckBoxChecked)
            {
                string fen = fenFromBoard.Get();
                gameSettingsContainer.SetSavedFen(fen);
                LogUi.Debug($"Board saved with: {fen}");
            }
        }
    }
}