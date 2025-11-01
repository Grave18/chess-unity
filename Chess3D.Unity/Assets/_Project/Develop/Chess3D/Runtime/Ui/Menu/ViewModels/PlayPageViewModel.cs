using Chess3D.Runtime.LobbyManagement;
using Chess3D.Runtime.Logic;
using Chess3D.Runtime.SceneManagement;
using Chess3D.Runtime.Settings;
using Cysharp.Threading.Tasks;
using LobbyManagement;
using Ui.Auxiliary;
using MvvmTool;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public partial class PlayPageViewModel : MonoBehaviour
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private OnlineSceneSwitcher onlineSceneSwitcher;

        public DelegateCommand PlayOfflineCommand { get; private set; }
        public DelegateCommand PlayWithComputerCommand { get; private set; }
        public DelegateCommand PlayOnlineCommand { get; private set; }

        public DelegateCommand StartLocalServerCommand { get; private set; }
        public DelegateCommand StartLocalClientCommand { get; private set; }

        private void Awake()
        {
            PlayOfflineCommand = new DelegateCommand(PlayOffline);
            PlayWithComputerCommand = new DelegateCommand(PlayWithComputer);
            PlayOnlineCommand = new DelegateCommand(PlayOnline);

            StartLocalServerCommand = new DelegateCommand(StartLocalServer);
            StartLocalClientCommand = new DelegateCommand(StartLocalClient);
        }

        private void PlayOffline(object obj)
        {
            gameSettingsContainer.SetupGameHumanVsHumanOffline();
            sceneLoader.LoadGame().Forget();
        }

        private void PlayWithComputer(object obj)
        {
            gameSettingsContainer.SetupGameHumanVsComputerOffline();
            sceneLoader.LoadGame().Forget();
        }

        private void PlayOnline(object obj)
        {
            LogUi.Debug("PlayOnline Clicked");
        }

        [RelayCommand]
        private void CreateLobby()
        {
            lobbyManager.CreateRoom();
        }

        [RelayCommand]
        private void FindGame()
        {
            // todo FindGame()
        }

        public void StartLocalServer(object obj)
        {
            onlineSceneSwitcher
                .SetupAndSwitchScene(PieceColor.White, isHost: true, isLocal: true)
                .Forget();
        }

        public void StartLocalClient(object obj)
        {
            onlineSceneSwitcher
                .SetupAndSwitchScene(PieceColor.Black, isHost: false, isLocal: true)
                .Forget();
        }
    }
}