using Logic.MenuStates;
using Notation;
using Settings;
using MvvmTool;
using Network;
using SceneManagement;
using Ui.Auxiliary;
using UnityEngine;
using UtilsCommon.Singleton;

namespace Ui.InGame.ViewModels
{
    [INotifyPropertyChanged]
    public partial class PopupViewModel : SingletonBehaviour<PopupViewModel>
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private Logic.Game game;
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
            PopupYesCommand = new RelayCommand();
            PopupNoCommand = new RelayCommand();
        }

        public void OpenConfigurablePopup(PopupSettings popupSettings)
        {
            IsPopupOpen = true;
            IsPopupNoButtonEnabled = popupSettings.IsPopupNoButtonEnabled;
            IsSaveCheckBoxEnabled = popupSettings.IsSaveCheckBoxEnabled;
            PopupText = popupSettings.PopupText;
            PopupYesButtonText = popupSettings.PopupYesButtonText;
            PopupNoButtonText = popupSettings.PopupNoButtonText;
            PopupYesCommand.Replace(popupSettings.PopupYesCommand);
            PopupNoCommand.Replace(popupSettings.PopupNoCommand);

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
            PopupNoButtonText = null;
            PopupYesCommand.Replace(ExitToMainMenu);
            PopupNoCommand.Replace(null);

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

        public void ExitToMainMenu()
        {
            SaveBoard();

            if (OnlineInstanceHandler.IsOnline)
            {
                ConnectionTerminator.DisconnectFromServer();
            }
            else
            {
                sceneLoader.LoadMainMenu().Forget();
            }
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

        public class PopupSettings
        {
            public bool IsPopupNoButtonEnabled = true;
            public bool IsSaveCheckBoxEnabled = false;
            public string PopupText = "n/a";
            public string PopupYesButtonText = "Yes";
            public string PopupNoButtonText = "No";
            public System.Action PopupYesCommand = null;
            public System.Action PopupNoCommand = null;
        }
    }
}