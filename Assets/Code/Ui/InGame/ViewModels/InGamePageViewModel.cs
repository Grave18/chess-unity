using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChessGame.Logic.MenuStates;
using GameAndScene;
using Network;
using Notation;
using Settings;
using MvvmTool;
using Ui.Auxiliary;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public class InGamePageViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private FenFromBoard fenFromBoard;

        public bool IsRematchButtonEnabled => OnlineInstanceHandler.IsOffline || (OnlineInstanceHandler.IsOnline && game.IsGameOver);
        public bool IsDrawButtonEnabled => OnlineInstanceHandler.IsOnline && !game.IsGameOver && game.IsMyTurn;
        public bool IsResignButtonEnabled => !game.IsGameOver && game.IsMyTurn;

        // Popup
        public DelegateCommand OpenResignPopupCommand { get; set; }
        public DelegateCommand OpenRematchPopupCommand { get; set; }
        public DelegateCommand OpenDrawPopupCommand { get; set; }
        public DelegateCommand OpenExitPopupCommand { get; set; }

        public DelegateCommand PopupYesCommand { get; set; }
        public DelegateCommand PopupNoCommand { get; set; }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetField(ref _isPopupOpen, value);
        }

        private string _popupText;
        public string PopupText
        {
            get => _popupText;
            set => SetField(ref _popupText, value);
        }

        private string _popupYesButtonText;
        public string PopupYesButtonText
        {
            get => _popupYesButtonText;
            set => SetField(ref _popupYesButtonText, value);
        }

        private string _popupNoButtonText;
        public string PopupNoButtonText
        {
            get => _popupNoButtonText;
            set => SetField(ref _popupNoButtonText, value);
        }

        private bool _isPopupNoButtonEnabled;
        public bool IsPopupNoButtonEnabled
        {
            get => _isPopupNoButtonEnabled;
            set => SetField(ref _isPopupNoButtonEnabled, value);
        }

        // Popup save checkbox
        private bool _isSaveCheckBoxEnabled;
        public bool IsSaveCheckBoxEnabled
        {
            get => _isSaveCheckBoxEnabled;
            set => SetField(ref _isSaveCheckBoxEnabled, value);
        }

        private bool _isSaveCheckBoxChecked;
        public bool IsSaveCheckBoxChecked
        {
            get => _isSaveCheckBoxChecked;
            set => SetField(ref _isSaveCheckBoxChecked, value);
        }

        private void Awake()
        {
            OpenResignPopupCommand = new DelegateCommand(OpenResignPopup);
            OpenRematchPopupCommand = new DelegateCommand(OpenRematchPopup);
            OpenDrawPopupCommand = new DelegateCommand(OpenDrawPopup);
            OpenExitPopupCommand = new DelegateCommand(OpenExitPopup);

            PopupYesCommand = new DelegateCommand();
            PopupNoCommand = new DelegateCommand();
        }

        private void OnEnable()
        {
            game.OnStart += UpdateButtonsIsEnabled;
            game.OnEnd += UpdateButtonsIsEnabled;
        }

        private void OnDisable()
        {
            game.OnStart -= UpdateButtonsIsEnabled;
            game.OnEnd -= UpdateButtonsIsEnabled;
        }

        private void UpdateButtonsIsEnabled()
        {
            OnPropertyChanged(nameof(IsRematchButtonEnabled));
            OnPropertyChanged(nameof(IsDrawButtonEnabled));
            OnPropertyChanged(nameof(IsResignButtonEnabled));
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

            MenuStateMachine.Instance.OpenPopup();
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

            MenuStateMachine.Instance.OpenPopup();
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

            MenuStateMachine.Instance.OpenPopup();
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

            MenuStateMachine.Instance.OpenPopup();
        }

        public void ClosePopupToPause(object obj)
        {
            IsPopupOpen = false;
            MenuStateMachine.Instance.ClosePopupToPause();
        }

        private void Rematch(object obj)
        {
            IsPopupOpen = false;
            game.Rematch();
            MenuStateMachine.Instance.ClosePopupToGame();
        }

        private void Draw(object obj)
        {
            IsPopupOpen = false;
            game.DrawByAgreement();
            MenuStateMachine.Instance.ClosePopupToGame();
        }

        private void Resign(object obj)
        {
            IsPopupOpen = false;
            game.Resign();
            MenuStateMachine.Instance.ClosePopupToGame();
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

        #region ViewModelImplimentation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion Implimentation
    }
}