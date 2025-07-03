#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Ui.Menu.ViewModels;
using UnityEngine;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows.Controls;
#endif

namespace Ui.Menu.UserControls
{
    public partial class FenTextBox : UserControl
    {
        public FenTextBox()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<FenUserControlViewModel>();
#endif
        }

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/UserControls/FenTextBox.xaml");
        }
#endif
    }
}
