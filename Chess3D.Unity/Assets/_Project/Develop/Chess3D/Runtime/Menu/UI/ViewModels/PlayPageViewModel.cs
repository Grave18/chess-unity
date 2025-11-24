using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Ai;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using LobbyManagement;
using MvvmTool;
using Ui.Auxiliary;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    public partial class PlayPageViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly LobbyManager _lobbyManager;
        private readonly SceneManager _sceneManager;

        public PlayPageViewModel(ISettingsService settingsService, LobbyManager lobbyManager, SceneManager sceneManager)
        {
            _settingsService = settingsService;
            _lobbyManager = lobbyManager;
            _sceneManager = sceneManager;
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
        private void PlayOnline()
        {
            LogUi.Debug("PlayOnline Clicked");
        }

        [RelayCommand]
        private void CreateLobby()
        {
            _lobbyManager.CreateRoom();
        }

        [RelayCommand]
        private void FindGame()
        {
            // TODO: implement FindGame()
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
    }
}