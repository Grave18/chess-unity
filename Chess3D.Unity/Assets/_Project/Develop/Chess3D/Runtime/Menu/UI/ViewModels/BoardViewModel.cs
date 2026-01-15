using System.Collections.ObjectModel;
using Chess3D.Runtime.Menu.UI.UnityGUI.BoardInMainMenu;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class BoardViewModel : IViewModel, ILoadUnit
    {
        public ObservableCollection<string> Pieces { get; set; }
        public ObservableCollection<string> Boards { get; set; }

        [ObservableProperty] private string _selectedPiece;
        [ObservableProperty] private string _selectedBoard;

        private readonly ISettingsContainer settingsContainer;
        private readonly VisualBoardSpawner _visualBoardSpawner;
        private readonly ViewNavigator _viewNavigator;

        public BoardViewModel(ISettingsContainer settingsContainer, VisualBoardSpawner visualBoardSpawner, ViewNavigator viewNavigator)
        {
            this.settingsContainer = settingsContainer;
            _visualBoardSpawner = visualBoardSpawner;
            _viewNavigator = viewNavigator;
        }

        public UniTask Load()
        {
            InitPieces();
            InitBoards();
            return UniTask.CompletedTask;
        }

        private void InitPieces()
        {
            _selectedPiece = settingsContainer.GameSettings.PiecesModelAddress;

            Pieces = new ObservableCollection<string>
            {
                "Prototype Pieces",
                "Day4 Pieces",
            };
        }

        private void InitBoards()
        {
            _selectedBoard = settingsContainer.GameSettings.BoardModelAddress;

            Boards = new ObservableCollection<string>
            {
                "Prototype Board",
                "Day4 Board",
            };
        }

        partial void OnSelectedBoardChanged(string value)
        {
            settingsContainer.GameSettings.BoardModelAddress = value;
            settingsContainer.Save();
            _visualBoardSpawner.SpawnBoard(value);
        }

        partial void OnSelectedPieceChanged(string value)
        {
            settingsContainer.GameSettings.PiecesModelAddress = value;
            settingsContainer.Save();
            _visualBoardSpawner.SpawnPieces(value);
        }

        [RelayCommand]
        private void Back()
        {
            _viewNavigator.GoBack();
        }
    }
}