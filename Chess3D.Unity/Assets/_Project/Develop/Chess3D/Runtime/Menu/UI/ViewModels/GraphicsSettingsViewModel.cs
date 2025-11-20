using System.Collections.ObjectModel;
using System.Linq;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using Ui.Auxiliary;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class GraphicsSettingsViewModel: ILoadUnit
    {
        public ObservableCollection<FullscreenModeUi> FullscreenModes { get; set; }
        public ObservableCollection<ResolutionUi> Resolutions { get; set; }
        public ObservableCollection<string> Qualities { get; set; }

        [ObservableProperty] private FullscreenModeUi _selectedFullscreenMode;
        [ObservableProperty] private ResolutionUi _selectedResolution;
        [ObservableProperty] private string _selectedQuality;

        private readonly ISettingsService _settingsService;

        private bool _isNeedToApplySettings;

        public GraphicsSettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
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

            _selectedFullscreenMode = FullscreenModes.FirstOrDefault(mode => mode.Index == _settingsService.S.GraphicsSettings.FullScreenMode);
        }

        private void InitResolution()
        {
            var availableResolutions = Screen.resolutions.
                Select(res => new ResolutionUi(res.width, res.height)).
                Distinct().
                Reverse();

            Resolutions = new ObservableCollection<ResolutionUi>(availableResolutions);
            _selectedResolution = Resolutions.FirstOrDefault(res => res.Width == _settingsService.S.GraphicsSettings.Width && res.Height == _settingsService.S.GraphicsSettings.Height);
        }

        private void InitQuality()
        {
            string[] levelsForPlatform = QualitySettings.names;

            Qualities = new ObservableCollection<string>(levelsForPlatform);
            _selectedQuality = Qualities.FirstOrDefault(quality =>
            {
                string name = _settingsService.S.GraphicsSettings.Quality < QualitySettings.names.Length
                    ? QualitySettings.names[_settingsService.S.GraphicsSettings.Quality]
                    : QualitySettings.names.FirstOrDefault();
                return quality == name;
            });
        }

        partial void OnSelectedFullscreenModeChanged(FullscreenModeUi value)
        {
            _settingsService.S.GraphicsSettings.FullScreenMode = value.Index;
            _isNeedToApplySettings = true;
            ApplySettingsCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedResolutionChanged(ResolutionUi value)
        {
            _settingsService.S.GraphicsSettings.Height = value.Height;
            _settingsService.S.GraphicsSettings.Width = value.Width;
            _isNeedToApplySettings = true;
            ApplySettingsCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedQualityChanged(string value)
        {
            _settingsService.S.GraphicsSettings.Quality = QualitySettings.names.ToList().IndexOf(value);
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
            int width = _settingsService.S.GraphicsSettings.Width;
            int height = _settingsService.S.GraphicsSettings.Height;
            int fullScreenMode = _settingsService.S.GraphicsSettings.FullScreenMode;
            int qualityIndex = _settingsService.S.GraphicsSettings.Quality;

            Screen.SetResolution(width, height, (FullScreenMode)fullScreenMode);
            QualitySettings.SetQualityLevel(qualityIndex);

            _isNeedToApplySettings = false;
            _settingsService.Save();
            ApplySettingsCommand.NotifyCanExecuteChanged();
        }
    }
}