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
using Ui.Menu.Pages;

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
#endif
        }

        public void Play_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<PlayPage>();
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<SettingsPage>();
        }

        private void Board_Click(object sender, RoutedEventArgs e)
        {
            GameMenuBase.Instance.ChangePage<BoardPage>();
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

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/Pages/InGamePage.xaml");
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
    }
}