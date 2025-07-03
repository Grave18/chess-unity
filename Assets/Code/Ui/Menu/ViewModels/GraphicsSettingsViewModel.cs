using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GameAndScene;
using Ui.Menu.Auxiliary;
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
                    LogUi.Debug($"Quality changed to {_selectedQuality}");
                }
            }
        }

        private void Awake()
        {
            ApplySettingsCommand = new DelegateCommand(CanExecuteApply, ApplySettings);

            SetFullScreenMode();
            SetResolution();
            SetQuality();
        }

        private bool CanExecuteApply(object arg)
        {
            return graphicsSettingsContainer.IsNeedToApplySettings();
        }

        private void ApplySettings(object obj)
        {
            graphicsSettingsContainer.ApplySettings();
            LogUi.Debug("Settings applied");
        }

        private void SetFullScreenMode()
        {
            FullscreenModes = new ObservableCollection<FullscreenModeUi>
            {
                new("Fullscreen", 0),
                new("Borderless", 1),
                new("Windowed", 3)
            };

            SelectedFullscreenMode = FullscreenModes.FirstOrDefault(IndexMatch) ;
            return;

            bool IndexMatch(FullscreenModeUi x) => x.Index == graphicsSettingsContainer.GetFullScreenMode();
        }

        private void SetResolution()
        {
            var availableResolutions = Screen.resolutions.
                Select(res => new ResolutionUi(res.width, res.height)).
                Distinct().
                Reverse();

            Resolutions = new ObservableCollection<ResolutionUi>(availableResolutions);
            SelectedResolution = graphicsSettingsContainer.GetResolution();
        }

        private void SetQuality()
        {
            string[] levelsForPlatform = QualitySettings.names;
            Qualities = new ObservableCollection<string>(levelsForPlatform);
            SelectedQuality = graphicsSettingsContainer.GetQualityString();
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