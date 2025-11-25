using System.Collections.ObjectModel;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Menu.UI.BoardInMainMenu;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class BoardPageViewModel : ILoadUnit
    {
        public ObservableCollection<string> Pieces { get; set; }
        public ObservableCollection<string> Boards { get; set; }

        [ObservableProperty] private string _selectedPiece;
        [ObservableProperty] private string _selectedBoard;

        private readonly ISettingsService _settingsService;
        private readonly VisualBoardSpawner _visualBoardSpawner;

        public BoardPageViewModel(ISettingsService settingsService, VisualBoardSpawner visualBoardSpawner)
        {
            _settingsService = settingsService;
            _visualBoardSpawner = visualBoardSpawner;
        }

        public UniTask Load()
        {
            InitPieces();
            InitBoards();
            return UniTask.CompletedTask;
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

        partial void OnSelectedBoardChanged(string value)
        {
            _settingsService.S.GameSettings.BoardModelAddress = value;
            _settingsService.Save();
            _visualBoardSpawner.SpawnBoard(value);
        }

        partial void OnSelectedPieceChanged(string value)
        {
            _settingsService.S.GameSettings.PiecesModelAddress = value;
            _settingsService.Save();
            _visualBoardSpawner.SpawnPieces(value);
        }
    }
}