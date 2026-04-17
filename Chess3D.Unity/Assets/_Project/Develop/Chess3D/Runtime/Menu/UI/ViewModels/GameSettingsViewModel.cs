using System;
using System.Collections.ObjectModel;
using System.Threading;
using Chess3D.Runtime.Core.AI;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class GameSettingsViewModel : ILoadUnit
    {
        public ObservableCollection<string> Times { get; set; }
        public ObservableCollection<string> Difficulties { get; set; }

        [ObservableProperty] private string _playerName;
        [ObservableProperty] private string _selectedTime;
        [ObservableProperty] private string _selectedDifficulty;

        private readonly ISettingsContainer settingsContainer;

        public GameSettingsViewModel(ISettingsContainer settingsContainer)
        {
            this.settingsContainer = settingsContainer;
        }

        public UniTask Load(CancellationToken cancellationToken = default)
        {
            InitName();
            InitTime();
            InitDifficulty();
            return UniTask.CompletedTask;
        }

        private void InitName()
        {
            _playerName = settingsContainer.GameSettings.PlayerWhite.Name;
        }

        private void InitTime()
        {
            Times = new ObservableCollection<string> { "1", "3", "5", "10", "20", "30", "40", "50", "60" };
            _selectedTime = settingsContainer.GameSettings.Time.x.ToString();
        }

        private void InitDifficulty()
        {
            var difficulties = Enum.GetNames(typeof(ComputerSkillLevel));
            Difficulties = new ObservableCollection<string>(difficulties);
            _selectedDifficulty = settingsContainer.GameSettings.PlayerBlack.ComputerSkillLevel.ToString();
        }

        partial void OnPlayerNameChanged(string value)
        {
            settingsContainer.GameSettings.PlayerWhite.Name = value;
            settingsContainer.Save();
        }

        partial void OnSelectedTimeChanged(string value)
        {
            if (int.TryParse(value, out int result))
            {
                settingsContainer.GameSettings.Time = new Vector2Int(result, 0);
                settingsContainer.Save();
            }
        }

        partial void OnSelectedDifficultyChanged(string value)
        {
            if (Enum.TryParse(value, out ComputerSkillLevel computerSkillLevel))
            {
                settingsContainer.GameSettings.PlayerBlack.ComputerSkillLevel = computerSkillLevel;
                settingsContainer.Save();
            }
        }
    }
}