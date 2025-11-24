using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public partial class FenUserControlViewModel : ILoadUnit
    {
        [ObservableProperty] private string _currentFen;

        private readonly ISettingsService _settingsService;

        public FenUserControlViewModel(ISettingsService settingsService)
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
        private void GetDefaultFen(object obj)
        {
            CurrentFen = _settingsService.S.GameSettings.CurrentFen;
        }

        [RelayCommand]
        private void GetSavedFen(object obj)
        {
            CurrentFen = _settingsService.S.GameSettings.CurrentFen;
        }

        partial void OnCurrentFenChanged(string value)
        {
            _settingsService.S.GameSettings.CurrentFen = value;
            _settingsService.Save();
        }
    }
}