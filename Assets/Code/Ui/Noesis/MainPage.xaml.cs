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

namespace Ui.Noesis
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
            GUI.LoadComponent(this, "Assets/Code/Ui/Noesis/MainPage.xaml");
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
#if NOESIS
            Debug.Log("Settings Clicked");
#endif
        }

        public void Exit_Click(object sender, RoutedEventArgs args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif NOESIS
            Application.Quit();
#endif
        }
    }
}