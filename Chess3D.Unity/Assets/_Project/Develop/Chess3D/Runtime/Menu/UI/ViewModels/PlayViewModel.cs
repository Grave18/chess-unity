using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Ai;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class PlayViewModel : IViewModel, ILoadUnit
    {
        [ObservableProperty] private bool _isOnlineEnabled;

        private readonly ISettingsService _settingsService;
        private readonly LobbyManager _lobbyManager;
        private readonly SceneManager _sceneManager;
        private readonly ViewNavigator _viewNavigator;

        public PlayViewModel(ISettingsService settingsService, LobbyManager lobbyManager, SceneManager sceneManager, ViewNavigator viewNavigator)
        {
            _settingsService = settingsService;
            _lobbyManager = lobbyManager;
            _sceneManager = sceneManager;
            _viewNavigator = viewNavigator;
        }

        public UniTask Load()
        {
            IsOnlineEnabled = _lobbyManager.CurrentProvider.IsLobbyCreationPossible;
            return UniTask.CompletedTask;
        }

        [RelayCommand]
        private void PlayOffline()
        {
            SetupGameHumanVsHumanOffline();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Core).Forget();
        }

        private void SetupGameHumanVsHumanOffline()
        {
            _settingsService.S.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
            _settingsService.S.GameSettings.PlayerBlack.PlayerType = PlayerType.Human;

            _settingsService.S.GameSettings.PlayerBlack.Name = "Player 2";
        }

        [RelayCommand]
        private void PlayWithComputer()
        {
            SetupGameHumanVsComputerOffline();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Core).Forget();
        }

        private void SetupGameHumanVsComputerOffline()
        {
            _settingsService.S.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
            _settingsService.S.GameSettings.PlayerBlack.PlayerType = PlayerType.Computer;

            ComputerSkillLevel skill = _settingsService.S.GameSettings.PlayerBlack.ComputerSkillLevel;
            _settingsService.S.GameSettings.PlayerBlack.Name = $"Computer({skill})";
        }

        [RelayCommand]
        private void CreateLobby()
        {
            _lobbyManager.CreateRoom();
        }

        [RelayCommand]
        public void StartLocalServer()
        {
            // TODO: Fix online scene switching
            // onlineSceneSwitcher
                // .SetupAndSwitchScene(PieceColor.White, isHost: true, isLocal: true)
                // .Forget();
        }

        [RelayCommand]
        public void StartLocalClient()
        {
            // onlineSceneSwitcher
                // .SetupAndSwitchScene(PieceColor.Black, isHost: false, isLocal: true)
                // .Forget();
        }

        [RelayCommand]
        private void Back()
        {
            _viewNavigator.GoBack();
        }
    }
}