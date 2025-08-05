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

        public RelayCommand PopupYesCommand { get; set; }
        public RelayCommand PopupNoCommand { get; set; }

        protected override void Awake()
        {
            base.Awake();
            PopupYesCommand = new RelayCommand(ClosePopupToPause);
            PopupNoCommand = new RelayCommand(ClosePopupToPause);
        }

        public void OpenRematchPopup()
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = true;
            IsSaveCheckBoxEnabled = false;
            PopupText = "Are you want to Rematch?";
            PopupYesButtonText = "Yes";
            PopupNoButtonText = "No";
            PopupYesCommand.Replace(Rematch);
            PopupNoCommand.Replace(ClosePopupToPause);

            menuStateMachine.OpenPopup();
        }

        public void OpenDrawPopup()
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = true;
            IsSaveCheckBoxEnabled = false;
            PopupText = "Are you want to Draw?";
            PopupYesButtonText = "Yes";
            PopupNoButtonText = "No";
            PopupYesCommand.Replace(Draw);
            PopupNoCommand.Replace(ClosePopupToPause);

            menuStateMachine.OpenPopup();
        }

        public void OpenResignPopup()
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = true;
            IsSaveCheckBoxEnabled = false;
            PopupText = "Are you want to Resign?";
            PopupYesButtonText = "Yes";
            PopupNoButtonText = "No";
            PopupYesCommand.Replace(Resign);
            PopupNoCommand.Replace(ClosePopupToPause);

            menuStateMachine.OpenPopup();
        }

        public void OpenExitPopup()
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = true;
            IsSaveCheckBoxEnabled = true;
            PopupText = "Are you want to Exit?";
            PopupYesButtonText = "Yes";
            PopupNoButtonText = "No";
            PopupYesCommand.Replace(ExitToMainMenu);
            PopupNoCommand.Replace(ClosePopupToPause);

            menuStateMachine.OpenPopup();
        }

        public void OpenReconnectPopup()
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = false;
            IsSaveCheckBoxEnabled = true;
            PopupText = "Wait for other player to reconnect...";
            PopupYesButtonText = "Exit to menu";
            PopupYesCommand.Replace(ExitToMainMenu);

            menuStateMachine.OpenPopup();
        }

        public void ClosePopupToPause()
        {
            IsPopupOpen = false;
            menuStateMachine.ClosePopupToPause();
        }

        public void ClosePopupToGame()
        {
            IsPopupOpen = false;
            menuStateMachine.ClosePopupToGame();
        }

        private void Rematch()
        {
            IsPopupOpen = false;
            game.Rematch();
            menuStateMachine.ClosePopupToGame();
        }

        private void Draw()
        {
            IsPopupOpen = false;
            game.DrawByAgreement();
            menuStateMachine.ClosePopupToGame();
        }

        private void Resign()
        {
            IsPopupOpen = false;
            game.Resign();
            menuStateMachine.ClosePopupToGame();
        }

        private void ExitToMainMenu()
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