using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Chess3D.Runtime.Settings;
using Ui.Auxiliary;
using MvvmTool;
using UnityEngine;

namespace Ui.Menu.ViewModels
{
    public class GraphicsSettingsViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private GraphicsSettingsContainer graphicsSettingsContainer;

        public DelegateCommand ApplySettingsCommand { get; set; }

        public ObservableCollection<FullscreenModeUi> FullscreenModes { get; set; }
        public ObservableCollection<ResolutionUi> Resolutions { get; set; }
        public ObservableCollection<string> Qualities { get; set; }

        private FullscreenModeUi _selectedFullscreenMode;
        public FullscreenModeUi SelectedFullscreenMode
        {
            get => _selectedFullscreenMode;
            set
            {
                if (SetField(ref _selectedFullscreenMode, value))
                {
                    graphicsSettingsContainer.SetFullScreenMode(_selectedFullscreenMode.Index);
                    ApplySettingsCommand.RaiseCanExecuteChanged();
                    LogUi.Debug($"Fullscreen mode changed to {_selectedFullscreenMode}");
                }
            }
        }

        private ResolutionUi _selectedResolution;
        public ResolutionUi SelectedResolution
        {
            get => _selectedResolution;
            set
            {
                if (SetField(ref _selectedResolution, value))
                {
                    graphicsSettingsContainer.SetResolution(_selectedResolution);
                    ApplySettingsCommand.RaiseCanExecuteChanged();
                    LogUi.Debug($"Resolution changed to {_selectedResolution}");
                }
            }
        }

        private string _selectedQuality;
        public string SelectedQuality
        {
            get => _selectedQuality;
            set
            {
                if (SetField(ref _selectedQuality, value))
                {
                    graphicsSettingsContainer.SetQuality(_selectedQuality);
                    ApplySettingsCommand.RaiseCanExecuteChanged();
                    LogUi.Debug($"Quality changed to {_selectedQuality}");
                }
            }
        }

        private void Awake()
        {
            ApplySettingsCommand = new DelegateCommand(CanExecuteApply, ApplySettings);

            InitFullScreenMode();
            InitResolution();
            InitQuality();
        }

        private bool CanExecuteApply(object arg)
        {
            bool isNeedToApplySettings = graphicsSettingsContainer.IsNeedToApplySettings();
            return isNeedToApplySettings;
        }

        private void ApplySettings(object obj)
        {
            graphicsSettingsContainer.ApplySettings();
            ApplySettingsCommand.RaiseCanExecuteChanged();
            LogUi.Debug("Settings applied");
        }

        private void InitFullScreenMode()
        {
            FullscreenModes = new ObservableCollection<FullscreenModeUi>
            {
                new("Fullscreen", 0),
                new("Borderless", 1),
                new("Windowed", 3),
            };

            _selectedFullscreenMode = FullscreenModes.FirstOrDefault(IsIndexMatch);
            return;

            bool IsIndexMatch(FullscreenModeUi x) => x.Index == graphicsSettingsContainer.GetFullScreenMode();
        }

        private void InitResolution()
        {
            var availableResolutions = Screen.resolutions.
                Select(res => new ResolutionUi(res.width, res.height)).
                Distinct().
                Reverse();

            Resolutions = new ObservableCollection<ResolutionUi>(availableResolutions);
            _selectedResolution = graphicsSettingsContainer.GetResolution();
        }

        private void InitQuality()
        {
            string[] levelsForPlatform = QualitySettings.names;
            Qualities = new ObservableCollection<string>(levelsForPlatform);
            _selectedQuality = graphicsSettingsContainer.GetQualityString();
        }

        #region ViewModelImplimentation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion Implimentation
    }
}