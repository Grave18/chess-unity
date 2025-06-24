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
    public partial class PlayPage : UserControl
    {
        public PlayPage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Noesis/PlayPage.xaml");
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(VsPlayer_Click))
            {
                ((Button)source).Click += VsPlayer_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(VsComputer_Click))
            {
                ((Button)source).Click += VsComputer_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(VsComputer_Click))
            {
                ((Button)source).Click += VsComputer_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Online_Click))
            {
                ((Button)source).Click += Online_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Back_Click))
            {
                ((Button)source).Click += Back_Click;
                return true;
            }

            return false;
        }
#endif

        private void OnInitialized(object sender, EventArgs args)
        {
            // this.DataContext = new ViewModel();
        }

        public void VsPlayer_Click(object sender, RoutedEventArgs args)
        {
            MainMenu.Instance.ChangePage<VsPlayerPage>();
        }

        public void VsComputer_Click(object sender, RoutedEventArgs args)
        {
            #if NOESIS
            Debug.Log("Vs Computer Clicked");
            #endif
        }

        public void Online_Click(object sender, RoutedEventArgs args)
        {
            #if NOESIS
            Debug.Log("Online Clicked");
            #endif
        }

        public void Back_Click(object sender, RoutedEventArgs args)
        {
            MainMenu.Instance.ChangePage<MainPage>();
        }
    }
}