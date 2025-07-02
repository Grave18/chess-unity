#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

using Ui.Menu.Auxiliary;

namespace Ui.Menu.Pages
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/Pages/MainPage.xaml");
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Play_Click))
            {
                ((Button)source).Click += Play_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Settings_Click))
            {
                ((Button)source).Click += Settings_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Board_Click))
            {
                ((Button)source).Click += Board_Click;
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

        private void OnInitialized(object sender, EventArgs args)
        {
            // this.DataContext = new ViewModel();
        }

        public void Play_Click(object sender, RoutedEventArgs args)
        {
            MainMenu.Instance.ChangePage<PlayPage>();
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            MainMenu.Instance.ChangePage<SettingsPage>();
        }

        private void Board_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Instance.ChangePage<BoardPage>();
        }

        public void Exit_Click(object sender, RoutedEventArgs args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif NOESIS
            Application.Quit();
#else
            Application.Current.Shutdown();
#endif
        }
    }
}