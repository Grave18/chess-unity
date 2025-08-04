using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameAndScene;
using Network;
using Notation;
using Settings;
using MvvmTool;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public class InGamePageViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private PopupViewModel popupViewModel;

        public bool IsRematchButtonEnabled => OnlineInstanceHandler.IsOffline || (OnlineInstanceHandler.IsOnline && game.IsGameOver);
        public bool IsDrawButtonEnabled => OnlineInstanceHandler.IsOnline && !game.IsGameOver && game.IsMyTurn;
        public bool IsResignButtonEnabled => !game.IsGameOver && game.IsMyTurn;

        // Popup Buttons
        public DelegateCommand OpenResignPopupCommand { get; set; }
        public DelegateCommand OpenRematchPopupCommand { get; set; }
        public DelegateCommand OpenDrawPopupCommand { get; set; }
        public DelegateCommand OpenExitPopupCommand { get; set; }

        private void Awake()
        {
            OpenResignPopupCommand = new DelegateCommand(OpenResignPopup);
            OpenRematchPopupCommand = new DelegateCommand(OpenRematchPopup);
            OpenDrawPopupCommand = new DelegateCommand(OpenDrawPopup);
            OpenExitPopupCommand = new DelegateCommand(OpenExitPopup);
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
            popupViewModel.OpenRematchPopup(obj);
        }

        public void OpenDrawPopup(object obj)
        {
            popupViewModel.OpenDrawPopup(obj);
        }

        public void OpenResignPopup(object obj)
        {
            popupViewModel.OpenResignPopup(obj);
        }

        public void OpenExitPopup(object obj)
        {
            popupViewModel.OpenExitPopup(obj);
        }

        public void ClosePopupToPause(object obj)
        {
            popupViewModel.ClosePopupToPause(obj);
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