using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.AI;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class PlayViewModel : IViewModel
    {
        [ObservableProperty] private bool _isOnlineEnabled;

        private readonly ISettingsContainer _settingsContainer;
        private readonly SceneManager _sceneManager;
        private readonly ViewNavigator _viewNavigator;

        public PlayViewModel(ISettingsContainer settingsContainer, SceneManager sceneManager,
            ViewNavigator viewNavigator)
        {
            _settingsContainer = settingsContainer;
            _sceneManager = sceneManager;
            _viewNavigator = viewNavigator;
        }

        [RelayCommand]
        private void PlayOffline()
        {
            SetupHumanVsHuman();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Core).Forget();
        }

        private void SetupHumanVsHuman()
        {
            _settingsContainer.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
            _settingsContainer.GameSettings.PlayerBlack.PlayerType = PlayerType.Human;
            _settingsContainer.GameSettings.PlayerBlack.Name = "Player 2";
        }

        [RelayCommand]
        private void PlayWithComputer()
        {
            SetupHumanVsComputer();
            _sceneManager.LoadScene(RuntimeConstants.Scenes.Core).Forget();
        }

        private void SetupHumanVsComputer()
        {
            _settingsContainer.GameSettings.PlayerWhite.PlayerType = PlayerType.Human;
            _settingsContainer.GameSettings.PlayerBlack.PlayerType = PlayerType.Computer;

            ComputerSkillLevel skill = _settingsContainer.GameSettings.PlayerBlack.ComputerSkillLevel;
            _settingsContainer.GameSettings.PlayerBlack.Name = $"Computer({skill})";
        }

        [RelayCommand]
        private void Back()
        {
            _viewNavigator.GoBack();
        }
    }
}