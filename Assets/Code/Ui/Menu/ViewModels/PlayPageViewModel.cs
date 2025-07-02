using GameAndScene;
using Ui.Menu.Auxiliary;
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

        private void Awake()
        {
            PlayOfflineCommand = new DelegateCommand(PlayOffline);
            PlayWithComputerCommand = new DelegateCommand(PlayWithComputer);
            PlayOnlineCommand = new DelegateCommand(PlayOnline);
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
    }
}