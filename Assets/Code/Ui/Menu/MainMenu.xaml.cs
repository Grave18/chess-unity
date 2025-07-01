#if UNITY_5_3_OR_NEWER
#define NOESIS
using System;
using Noesis;
using UnityEngine;
using EventArgs = Noesis.EventArgs;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

namespace Ui.Menu
{
    public partial class MainMenu : UserControl
    {
        public static MainMenu Instance { get; private set; }

        public MainMenu()
        {
            Initialized += OnInitialized;
            InitializeComponent();
            Instance = this;
        }

#if NOESIS
        private ContentControl ContentControl { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/MainMenu.xaml");

            ContentControl = FindName("ContentControl") as ContentControl;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Play_Click))
            {
                ((Button)source).Click += Play_Click;
                return true;
            }

            return false;
        }
#endif

        public void ChangePage<T>() where T : UserControl
        {
            ContentControl.Content = Activator.CreateInstance(typeof(T));
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            // this.DataContext = new ViewModel();
        }

        public void Play_Click(object sender, RoutedEventArgs args)
        {
 #if NOESIS
            Debug.Log("Play Clicked");
 #endif
        }
    }
}