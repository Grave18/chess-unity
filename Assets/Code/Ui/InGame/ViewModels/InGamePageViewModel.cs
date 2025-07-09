using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameAndScene;
using Network;
using Ui.Auxiliary;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public class InGamePageViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private ChessGame.Logic.Game game;

        public DelegateCommand RematchCommand { get; set; }
        public DelegateCommand DrawCommand { get; set; }
        public DelegateCommand ResignCommand { get; set; }
        public DelegateCommand ExitToMainMenuCommand { get; set; }

        public bool IsRematchButtonEnabled => OnlineInstanceHandler.IsOffline || (OnlineInstanceHandler.IsOnline && game.IsGameOver);
        public bool IsDrawButtonEnabled => OnlineInstanceHandler.IsOnline && !game.IsGameOver && game.IsMyTurn;
        public bool IsResignButtonEnabled => !game.IsGameOver && game.IsMyTurn;

        private void Awake()
        {
            RematchCommand = new DelegateCommand(Rematch);
            DrawCommand = new DelegateCommand(Draw);
            ResignCommand = new DelegateCommand(Resign);
            ExitToMainMenuCommand = new DelegateCommand(ExitToMainMenu);
        }

        private void Rematch(object obj)
        {
            game.Rematch();
        }

        private void Draw(object obj)
        {
            game.DrawByAgreement();
        }

        private void Resign(object obj)
        {
            game.Resign();
        }

        private void ExitToMainMenu(object obj)
        {
            sceneLoader.LoadMainMenu();
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