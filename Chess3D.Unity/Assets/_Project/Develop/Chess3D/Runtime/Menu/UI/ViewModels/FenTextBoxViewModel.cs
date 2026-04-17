using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Utilities.Logging;
using System.Threading;
using Cysharp.Threading.Tasks;
using MvvmTool;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Menu.UI.ViewModels
{
    [Preserve]
    [INotifyPropertyChanged]
    public sealed partial class FenTextBoxViewModel : ILoadUnit
    {
        [ObservableProperty] private string _currentFen;

        private readonly ISettingsContainer settingsContainer;

        public FenTextBoxViewModel(ISettingsContainer settingsContainer)
        {
            this.settingsContainer = settingsContainer;
        }

        public UniTask Load(CancellationToken cancellationToken = default)
        {
            InitFen();
            return UniTask.CompletedTask;
        }

        private void InitFen()
        {
            _currentFen = settingsContainer.GameSettings.CurrentFen;
        }

        [RelayCommand]
        private void GetDefaultFen()
        {
            CurrentFen = settingsContainer.GameSettings.DefaultFen;
        }

        [RelayCommand]
        private void GetSavedFen()
        {
            CurrentFen = settingsContainer.GameSettings.SavedFen;
        }

        partial void OnCurrentFenChanged(string value)
        {
            if (FenValidator.IsValid(value, out string errorMessage))
            {
                settingsContainer.GameSettings.CurrentFen = value;
                settingsContainer.Save();
            }
            else
            {
                CurrentFen = settingsContainer.GameSettings.CurrentFen;
                Log.Editor.W(errorMessage);
            }
        }
    }
}