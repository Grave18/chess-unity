using Chess3D.Runtime.Bootstrap.Settings;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class FenTextBoxViewModel : ILoadUnit
    {
        [ObservableProperty] private string _currentFen;

        private readonly ISettingsService _settingsService;

        public FenTextBoxViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public UniTask Load()
        {
            InitFen();
            return UniTask.CompletedTask;
        }

        private void InitFen()
        {
            _currentFen = _settingsService.S.GameSettings.CurrentFen;
        }

        [RelayCommand]
        private void GetDefaultFen()
        {
            CurrentFen = _settingsService.S.GameSettings.DefaultFen;
        }

        [RelayCommand]
        private void GetSavedFen()
        {
            CurrentFen = _settingsService.S.GameSettings.SavedFen;
        }

        partial void OnCurrentFenChanged(string value)
        {
            if (FenValidator.IsValid(value, out string errorMessage))
            {
                _settingsService.S.GameSettings.CurrentFen = value;
                _settingsService.Save();
            }
            else
            {
                CurrentFen = _settingsService.S.GameSettings.CurrentFen;
                Log.Editor.W(errorMessage);
            }
        }
    }
}