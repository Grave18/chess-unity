using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Logic.MenuStates;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using PurrNet;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class PopupViewModel
    {
        [ObservableProperty] private bool _isPopupOpen;
        [ObservableProperty] private string _popupText;
        [ObservableProperty] private string _popupYesButtonText;
        [ObservableProperty] private string _popupNoButtonText;
        [ObservableProperty] private bool _isPopupNoButtonEnabled;
        [ObservableProperty] private bool _isSaveCheckBoxEnabled;
        [ObservableProperty] private bool _isSaveCheckBoxChecked;

        private readonly FenFromBoard _fenFromBoard;
        private readonly MenuStateMachine _menuStateMachine;
        private readonly SceneManager _sceneManager;
        private readonly ISettingsContainer settingsContainer;

        public RelayCommand PopupYesCommand { get; set; } = new();
        public RelayCommand PopupNoCommand { get; set; } = new();

        public PopupViewModel(FenFromBoard fenFromBoard, MenuStateMachine menuStateMachine, SceneManager sceneManager, ISettingsContainer settingsContainer)
        {
            _fenFromBoard = fenFromBoard;
            _menuStateMachine = menuStateMachine;
            _sceneManager = sceneManager;
            this.settingsContainer = settingsContainer;
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

            _menuStateMachine.OpenPopup();
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

            _menuStateMachine.OpenPopup();
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

            _menuStateMachine.OpenPopup();
        }

        public void ClosePopupToPause()
        {
            IsPopupOpen = false;
            _menuStateMachine.ClosePopupToPause();
        }

        public void ClosePopupToGame()
        {
            IsPopupOpen = false;
            _menuStateMachine.ClosePopupToGame();
        }

        public void ExitToMainMenu()
        {
            SaveBoard();

            if (OnlineInstanceHandler.IsOnline)
            {
                InstanceHandler.NetworkManager?.StopClient();
            }
            else
            {
                _sceneManager.LoadScene(RuntimeConstants.Scenes.Menu).Forget();
            }
        }

        private void SaveBoard()
        {
            if (IsSaveCheckBoxChecked)
            {
                string fen = _fenFromBoard.Get();
                settingsContainer.S.GameSettings.SavedFen = fen;
                settingsContainer.Save();
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