#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Ui.Menu.ViewModels;
using UnityEngine;
using Grid = Noesis.Grid;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
    public partial class PlayPage : UserControl
    {
        public PlayPage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<PlayPageViewModel>();
            Root.DataContext = Object.FindAnyObjectByType<GameSettingsViewModel>();
#endif
        }

        public void Back_Click(object sender, RoutedEventArgs args)
        {
            Auxiliary.GameMenuBase.Instance.ChangePage<MainPage>();
        }

#if NOESIS
        public Grid Root { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/Pages/PlayPage.xaml");

            Root = FindName("Root") as Grid;
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