using System.ComponentModel;
using GameAndScene;
using Network;
using Notation;
using Settings;
using MvvmTool;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public partial class InGamePageViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private ChessGame.Logic.Game game;
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private FenFromBoard fenFromBoard;
        [SerializeField] private PopupViewModel popupViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand OpenRematchPopupCommand { get; set; }
        public DelegateCommand OpenDrawPopupCommand { get; set; }
        public DelegateCommand OpenResignPopupCommand { get; set; }

        private void Awake()
        {
            OpenRematchPopupCommand = new DelegateCommand(OpenRematchPopup_CanExecute, OpenRematchPopup);
            OpenDrawPopupCommand = new DelegateCommand(OpenDrawPopup_CanExecute, OpenDrawPopup);
            OpenResignPopupCommand = new DelegateCommand(OpenResignPopup_CanExecute, OpenResignPopup);
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

        [RelayCommand]
        public void OpenExitPopup(object obj)
        {
            popupViewModel.OpenExitPopup(obj);
        }

        public bool OpenRematchPopup_CanExecute(object obj)
        {
            return OnlineInstanceHandler.IsOffline || (OnlineInstanceHandler.IsOnline && game.IsGameOver);
        }

        public bool OpenDrawPopup_CanExecute(object obj)
        {
            return OnlineInstanceHandler.IsOnline && !game.IsGameOver && game.IsMyTurn;
        }

        public bool OpenResignPopup_CanExecute(object obj)
        {
            return !game.IsGameOver && game.IsMyTurn;
        }
    }
}