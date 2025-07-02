#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Ui.Menu.ViewModels;
using UnityEngine;
using GUI = Noesis.GUI;
using Grid = Noesis.Grid;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

using System.Windows.Input;

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

        // Move focus next when Enter key pressed
        private void FenTextBox1_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var element = sender as UIElement;
                if (element != null)
                {
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
        }

#if NOESIS
        private Grid Root { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/UserControls/FenTextBox.xaml");

            Root = FindName("Root") as Grid;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "KeyUp" && handlerName == nameof(FenTextBox1_OnKeyUp))
            {
                ((TextBox)source).KeyUp += FenTextBox1_OnKeyUp;
                return true;
            }

            return false;
        }
#endif
    }
}
