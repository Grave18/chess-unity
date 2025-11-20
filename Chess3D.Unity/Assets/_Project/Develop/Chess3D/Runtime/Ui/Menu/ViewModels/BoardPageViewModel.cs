using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Chess3D.Runtime;
using Chess3D.Runtime.Bootstrap.Settings;
using Ui.Auxiliary;
using Ui.BoardInMainMenu;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class BoardPageViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private VisualBoardSpawner visualBoardSpawner;

        // TODO: Add DI dependency
        private SettingsService _settingsService;

        public ObservableCollection<string> Pieces { get; set; } = new();
        public ObservableCollection<string> Boards { get; set; } = new();

        private string _selectedPiece;
        public string SelectedPiece
        {
            get => _selectedPiece;
            set
            {
                if (SetField(ref _selectedPiece, value))
                {
                    _settingsService.S.GameSettings.PiecesModelAddress = value;
                    _settingsService.Save();
                    visualBoardSpawner.SpawnPieces(value);

                    LogUi.Debug($"{nameof(SelectedPiece)} changed to {value}");
                }
            }
        }

        private string _selectedBoard;

        public string SelectedBoard
        {
            get => _selectedBoard;
            set
            {
                if (SetField(ref _selectedBoard, value))
                {
                    _settingsService.S.GameSettings.BoardModelAddress = value;
                    _settingsService.Save();
                    visualBoardSpawner.SpawnBoard(value);

                    LogUi.Debug($"{nameof(SelectedBoard)} changed to {value}");
                }
            }
        }

        private void Awake()
        {
            InitPieces();
            InitBoards();
        }

        private void InitPieces()
        {
            _selectedPiece = _settingsService.S.GameSettings.PiecesModelAddress;

            Pieces = new ObservableCollection<string>
            {
                "Prototype Pieces",
                "Day4 Pieces",
            };
        }

        private void InitBoards()
        {
            _selectedBoard = _settingsService.S.GameSettings.BoardModelAddress;

            Boards = new ObservableCollection<string>
            {
                "Prototype Board",
                "Day4 Board",
            };
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