#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;
using Ui.InGame.ViewModels;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
#endif

using Ui.Auxiliary;

namespace Ui.InGame.Pages
{
    public partial class InGamePage : UserControl
    {
        private CheckBox _saveCheckBox;

        public InGamePage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<InGamePageViewModel>();
            ResumeButton.DataContext = Object.FindAnyObjectByType<InGameMenuViewModel>();
#endif

            // Add to InGame Page ability to handle keyboard Esc and Enter
            this.KeyUp += OnKeyUp;
        }

        /// Handle popup buttons handling by code
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // if (Popup.IsOpen)
            // {
            //     if (e.Key == Key.Escape)
            //     {
            //         PopupNoButton.Command.Execute(null);
            //         e.Handled = true;
            //     }
            //     else if (e.Key == Key.Enter)
            //     {
            //         PopupYesButton.Command.Execute(null);
            //         e.Handled = true;
            //     }
            // }
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<InGameSettingsPage>();
        }

#if NOESIS

        private Popup Popup { get; set; }
        private Button PopupYesButton { get; set; }
        private Button PopupNoButton { get; set; }
        private TextBlock PopupText { get; set; }
        private Button ResumeButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/Pages/InGamePage.xaml");

            ResumeButton = FindName("ResumeButton") as Button;

            Popup = FindName("Popup") as Popup;
            PopupText = FindName("PopupText") as TextBlock;
            PopupYesButton = FindName("PopupYesButton") as Button;
            PopupNoButton = FindName("PopupNoButton") as Button;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Settings_Click))
            {
                ((Button)source).Click += Settings_Click;
                return true;
            }

            return false;
        }

#endif
    }
}