using ChessGame.Logic;
using GameAndScene;
using Settings;
using Ui.Auxiliary;
using MvvmTool;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class PlayPageViewModel : MonoBehaviour
    {
        [SerializeField] private GameSettingsContainer gameSettingsContainer;
        [SerializeField] private SceneLoader sceneLoader;

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
            gameSettingsContainer.SetupGameOffline();
            sceneLoader.LoadGame();
        }

        private void PlayWithComputer(object obj)
        {
            gameSettingsContainer.SetupGameWithComputer();
            sceneLoader.LoadGame();
        }

        private void PlayOnline(object obj)
        {
            LogUi.Debug("PlayOnline Clicked");
        }

        public void StartLocalServer(object obj)
        {
            StartLocal(PieceColor.White, true);
        }

        public void StartLocalClient(object obj)
        {
            StartLocal(PieceColor.Black, false);
        }

        private void StartLocal(PieceColor playerColor, bool isLocalhostServer)
        {
            gameSettingsContainer.SetupGameOnline(playerColor);
            GameSettingsContainer.IsLocalhostServer = isLocalhostServer;

            sceneLoader.LoadOnlineLocalhost();
        }
    }
}