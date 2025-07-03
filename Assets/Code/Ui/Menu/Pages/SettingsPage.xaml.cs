#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Ui.Menu.ViewModels;
using UnityEngine;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

namespace Ui.Menu.Pages
{
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            #if NOESIS
                DataContext = Object.FindAnyObjectByType<SettingsPageViewModel>();
                GameSettingsTab.DataContext = Object.FindAnyObjectByType<GameSettingsViewModel>();
                GraphicsSettingsTab.DataContext = Object.FindAnyObjectByType<GraphicsSettingsViewModel>();
            #endif
        }

        public void Back_Click(object sender, RoutedEventArgs args)
        {
            MainMenu.Instance.ChangePage<MainPage>();
        }

#if NOESIS
        private TabItem GameSettingsTab { get; set; }
        private TabItem GraphicsSettingsTab { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/Pages/SettingsPage.xaml");

            GameSettingsTab = FindName("GameSettingsTab") as TabItem;
            GraphicsSettingsTab = FindName("GraphicsSettingsTab") as TabItem;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Back_Click))
            {
                ((Button)source).Click += Back_Click;
                return true;
            }

            return false;
        }
#endif
    }
}