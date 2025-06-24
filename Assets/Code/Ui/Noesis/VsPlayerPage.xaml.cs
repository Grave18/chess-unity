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
    public partial class VsPlayerPage : UserControl
    {
        public VsPlayerPage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Noesis/VsPlayerPage.xaml");
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(StartMatch_Click))
            {
                ((Button)source).Click += StartMatch_Click;
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

        public void StartMatch_Click(object sender, RoutedEventArgs args)
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