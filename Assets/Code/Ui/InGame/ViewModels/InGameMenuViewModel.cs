using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ui.Auxiliary;
using UnityEngine;

namespace Ui.InGame.ViewModels
{
    public class InGameMenuViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        [SerializeField] private GameObject gameCanvas;

        public DelegateCommand OpenCloseCommand { get; set; }

        private bool _isOpened;
        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                if (SetField(ref _isOpened, value))
                {
                    OnOpenClosed(value);
                }
            }
        }

        private void OnOpenClosed(bool value)
        {
            gameCanvas.SetActive(!value);
            Debug.Log($"{nameof(IsOpened)} is changed to  {value}");
        }

        private void Awake()
        {
            OpenCloseCommand = new DelegateCommand(OpenClose);
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                OpenClose(null);
            }
        }

        private void OpenClose(object obj)
        {
            IsOpened = !IsOpened;
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