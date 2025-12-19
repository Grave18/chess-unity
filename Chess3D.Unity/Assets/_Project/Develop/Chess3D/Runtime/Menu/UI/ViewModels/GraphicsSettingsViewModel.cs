using System.Collections.ObjectModel;
using System.Linq;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using Chess3D.Runtime.Menu.UI.Auxiliary;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class GraphicsSettingsViewModel: ILoadUnit
    {
        public ObservableCollection<FullscreenModeUi> FullscreenModes { get; set; }
        public ObservableCollection<ResolutionUi> Resolutions { get; set; }
        public ObservableCollection<string> Qualities { get; set; }

        [ObservableProperty] private FullscreenModeUi _selectedFullscreenMode;
        [ObservableProperty] private ResolutionUi _selectedResolution;
        [ObservableProperty] private string _selectedQuality;

        private readonly ISettingsContainer settingsContainer;

        private bool _isNeedToApplySettings;

        public GraphicsSettingsViewModel(ISettingsContainer settingsContainer)
        {
            this.settingsContainer = settingsContainer;
        }

        public UniTask Load()
        {
            InitFullScreenMode();
            InitResolution();
            InitQuality();
            return UniTask.CompletedTask;
        }

        private void InitFullScreenMode()
        {
            FullscreenModes = new ObservableCollection<FullscreenModeUi>
            {
                new("Fullscreen", 0),
                new("Borderless", 1),
                new("Windowed", 3),
            };

            _selectedFullscreenMode = FullscreenModes.FirstOrDefault(mode => mode.Index == settingsContainer.S.GraphicsSettings.FullScreenMode);
        }

        private void InitResolution()
        {
            var availableResolutions = Screen.resolutions.
                Select(res => new ResolutionUi(res.width, res.height)).
                Distinct().
                Reverse();

            Resolutions = new ObservableCollection<ResolutionUi>(availableResolutions);
            _selectedResolution = Resolutions.FirstOrDefault(res => res.Width == settingsContainer.S.GraphicsSettings.Width && res.Height == settingsContainer.S.GraphicsSettings.Height);
        }

        private void InitQuality()
        {
            string[] levelsForPlatform = QualitySettings.names;

            Qualities = new ObservableCollection<string>(levelsForPlatform);
            _selectedQuality = Qualities.FirstOrDefault(quality =>
            {
                string name = settingsContainer.S.GraphicsSettings.Quality < QualitySettings.names.Length
                    ? QualitySettings.names[settingsContainer.S.GraphicsSettings.Quality]
                    : QualitySettings.names.FirstOrDefault();
                return quality == name;
            });
        }

        partial void OnSelectedFullscreenModeChanged(FullscreenModeUi value)
        {
            settingsContainer.S.GraphicsSettings.FullScreenMode = value.Index;
            _isNeedToApplySettings = true;
            ApplySettingsCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedResolutionChanged(ResolutionUi value)
        {
            settingsContainer.S.GraphicsSettings.Height = value.Height;
            settingsContainer.S.GraphicsSettings.Width = value.Width;
            _isNeedToApplySettings = true;
            ApplySettingsCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedQualityChanged(string value)
        {
            settingsContainer.S.GraphicsSettings.Quality = QualitySettings.names.ToList().IndexOf(value);
            _isNeedToApplySettings = true;
            ApplySettingsCommand.NotifyCanExecuteChanged();
        }

        private bool CanExecuteApply()
        {
            return _isNeedToApplySettings;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteApply))]
        private void ApplySettings()
        {
            int width = settingsContainer.S.GraphicsSettings.Width;
            int height = settingsContainer.S.GraphicsSettings.Height;
            int fullScreenMode = settingsContainer.S.GraphicsSettings.FullScreenMode;
            int qualityIndex = settingsContainer.S.GraphicsSettings.Quality;

            Screen.SetResolution(width, height, (FullScreenMode)fullScreenMode);
            QualitySettings.SetQualityLevel(qualityIndex);

            _isNeedToApplySettings = false;
            settingsContainer.Save();
            ApplySettingsCommand.NotifyCanExecuteChanged();
        }
    }
}