using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.AI;
using Chess3D.Runtime.Core.Logic;
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
            SetupHumanVsHuman();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Core).Forget();
        }

        private void SetupHumanVsHuman()
        {
            _configContainer.OnlineConfig.ConnectionType = ConnectionType.Offline;
            _configContainer.OnlineConfig.IsLocal = false;
            _configContainer.OnlineConfig.AuthorityType = AuthorityType.None;

            _settingsContainer.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
            _settingsContainer.GameSettings.PlayerBlack.PlayerType = PlayerType.Human;
            _settingsContainer.GameSettings.PlayerBlack.Name = "Player 2";
        }

        [RelayCommand]
        private void PlayWithComputer()
        {
            SetupHumanVsComputerOffline();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Core).Forget();
        }

        private void SetupHumanVsComputerOffline()
        {
            _configContainer.OnlineConfig.ConnectionType = ConnectionType.Offline;
            _configContainer.OnlineConfig.IsLocal = false;
            _configContainer.OnlineConfig.AuthorityType = AuthorityType.None;

            _settingsContainer.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
            _settingsContainer.GameSettings.PlayerBlack.PlayerType = PlayerType.Computer;

            ComputerSkillLevel skill = _settingsContainer.GameSettings.PlayerBlack.ComputerSkillLevel;
            _settingsContainer.GameSettings.PlayerBlack.Name = $"Computer({skill})";
        }

        [RelayCommand]
        private void CreateLobby()
        {
            _lobbyManager.CreateRoom();
        }

        [RelayCommand]
        public void StartLocalServer()
        {
            SetupHumanHost();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Online).Forget();
        }

        private void SetupHumanHost()
        {
            _configContainer.OnlineConfig.ConnectionType = ConnectionType.Online;
            _configContainer.OnlineConfig.IsLocal = true;
            _configContainer.OnlineConfig.AuthorityType = AuthorityType.Host;

            _settingsContainer.GameSettings.PlayerColor = PieceColor.White;
            _settingsContainer.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
        }

        [RelayCommand]
        public void StartLocalClient()
        {
            SetupHumanClient();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Online).Forget();
        }

        private void SetupHumanClient()
        {
            _configContainer.OnlineConfig.ConnectionType = ConnectionType.Online;
            _configContainer.OnlineConfig.IsLocal = true;
            _configContainer.OnlineConfig.AuthorityType = AuthorityType.Client;

            _settingsContainer.GameSettings.PlayerColor = PieceColor.Black;
            _settingsContainer.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
        }

        [RelayCommand]
        private void Back()
        {
            _viewNavigator.GoBack();
        }
    }
}