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
#endif

using Ui.Auxiliary;

namespace Ui.InGame.Pages
{
    public partial class InGamePage : UserControl
    {
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
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<InGameSettingsPage>();
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            ExitPopup.IsOpen = false;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitPopup.IsOpen = true;
        }

#if NOESIS
        private Popup ExitPopup { get; set; }
        private Button ResumeButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/Pages/InGamePage.xaml");

            ResumeButton = FindName("ResumeButton") as Button;
            ExitPopup = FindName("ExitPopup") as Popup;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Settings_Click))
            {
                ((Button)source).Click += Settings_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(ClosePopup_Click))
            {
                ((Button)source).Click += ClosePopup_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Exit_Click))
            {
                ((Button)source).Click += Exit_Click;
                return true;
            }

            return false;
        }
#endif
    }
}