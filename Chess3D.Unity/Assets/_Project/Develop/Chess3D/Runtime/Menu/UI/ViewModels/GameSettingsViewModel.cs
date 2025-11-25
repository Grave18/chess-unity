using System;
using System.Collections.ObjectModel;
using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Ai;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class GameSettingsViewModel : ILoadUnit
    {
        public ObservableCollection<string> Times { get; set; }
        public ObservableCollection<string> Difficulties { get; set; }

        [ObservableProperty] private string _playerName;
        [ObservableProperty] private string _selectedTime;
        [ObservableProperty] private string _selectedDifficulty;

        private readonly ISettingsService _settingsService;

        public GameSettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public UniTask Load()
        {
            InitName();
            IntiTime();
            InitDifficulty();
            return UniTask.CompletedTask;
        }

        private void InitName()
        {
            _playerName = _settingsService.S.GameSettings.PlayerWhite.Name;
        }

        private void IntiTime()
        {
            Times = new ObservableCollection<string> { "1", "3", "5", "10", "20", "30", "40", "50", "60" };
            _selectedTime = _settingsService.S.GameSettings.Time.x.ToString();
        }

        private void InitDifficulty()
        {
            var difficulties = Enum.GetNames(typeof(ComputerSkillLevel));
            Difficulties = new ObservableCollection<string>(difficulties);
            _selectedDifficulty = _settingsService.S.GameSettings.PlayerBlack.ComputerSkillLevel.ToString();
        }

        partial void OnPlayerNameChanged(string value)
        {
            _settingsService.S.GameSettings.PlayerWhite.Name = value;
        }

        partial void OnSelectedTimeChanged(string value)
        {
            if (int.TryParse(value, out int result))
            {
                _settingsService.S.GameSettings.Time = new Vector2Int(result, 0);
                _settingsService.Save();
            }
        }

        partial void OnSelectedDifficultyChanged(string value)
        {
            if (Enum.TryParse(value, out ComputerSkillLevel computerSkillLevel))
            {
                _settingsService.S.GameSettings.PlayerBlack.ComputerSkillLevel = computerSkillLevel;
                _settingsService.Save();
            }
        }
    }
}