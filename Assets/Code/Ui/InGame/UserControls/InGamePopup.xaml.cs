#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using Ui.InGame.ViewModels;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows.Controls;
using System.Windows.Input;
#endif

namespace Ui.InGame.UserControls
{
    public partial class InGamePopup : UserControl
    {
        public InGamePopup()
        {
            Initialized += OnInitialized;
            InitializeComponent();

            KeyDown += OnKeyDown;
        }

        private void OnInitialized(object sender, EventArgs e)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<PopupViewModel>();
#endif
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {

            }
            else if (e.Key == Key.Enter)
            {

            }
        }

#if NOESIS

    private void InitializeComponent()
    {
         GUI.LoadComponent(this, "Assets/Code/Ui/InGame/UserControls/InGamePopup.xaml");
    }

#endif
    }
}
