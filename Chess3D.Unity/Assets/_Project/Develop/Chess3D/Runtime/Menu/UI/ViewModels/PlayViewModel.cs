using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.AI;
using Chess3D.Runtime.Menu.LobbyManagement;
using Chess3D.Runtime.Online;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class PlayViewModel : IViewModel, ILoadUnit
    {
        [ObservableProperty] private bool _isOnlineEnabled;

        private readonly ISettingsContainer _settingsContainer;
        private readonly ConfigContainer _configContainer;
        private readonly LobbyManager _lobbyManager;
        private readonly SceneManager _sceneManager;
        private readonly ViewNavigator _viewNavigator;

        public PlayViewModel(ISettingsContainer settingsContainer, LobbyManager lobbyManager, SceneManager sceneManager,
            ViewNavigator viewNavigator, ConfigContainer configContainer)
        {
            _settingsContainer = settingsContainer;
            _lobbyManager = lobbyManager;
            _sceneManager = sceneManager;
            _viewNavigator = viewNavigator;
            _configContainer = configContainer;
        }

        public UniTask Load()
        {
            IsOnlineEnabled = _lobbyManager.CurrentProvider.IsLobbyCreationPossible;
            return UniTask.CompletedTask;
        }

        [RelayCommand]
        private void PlayOffline()
        {
            _settingsContainer.S.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
            _settingsContainer.S.GameSettings.PlayerBlack.PlayerType = PlayerType.Human;
            _settingsContainer.S.GameSettings.PlayerBlack.Name = "Player 2";

            _sceneManager.LoadScene(RuntimeConstants.Scenes.Core, true).Forget();
        }

        [RelayCommand]
        private void PlayWithComputer()
        {
            SetupGameHumanVsComputerOffline();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Core, true).Forget();
        }

        private void SetupGameHumanVsComputerOffline()
        {
            _settingsContainer.S.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
            _settingsContainer.S.GameSettings.PlayerBlack.PlayerType = PlayerType.Computer;

            ComputerSkillLevel skill = _settingsContainer.S.GameSettings.PlayerBlack.ComputerSkillLevel;
            _settingsContainer.S.GameSettings.PlayerBlack.Name = $"Computer({skill})";
        }

        [RelayCommand]
        private void CreateLobby()
        {
            _lobbyManager.CreateRoom();
        }

        [RelayCommand]
        public void StartLocalServer()
        {
            _configContainer.OnlineConfig.ConnectionType = ConnectionType.Online;
            _configContainer.OnlineConfig.IsLocal = true;
            _configContainer.OnlineConfig.AuthorityType = AuthorityType.Host;

            _sceneManager.LoadScene(RuntimeConstants.Scenes.Online, true).Forget();
        }

        [RelayCommand]
        public void StartLocalClient()
        {
            _configContainer.OnlineConfig.ConnectionType = ConnectionType.Online;
            _configContainer.OnlineConfig.IsLocal = true;
            _configContainer.OnlineConfig.AuthorityType = AuthorityType.Client;

            _sceneManager.LoadScene(RuntimeConstants.Scenes.Online, true).Forget();
        }

        [RelayCommand]
        private void Back()
        {
            _viewNavigator.GoBack();
        }
    }
}