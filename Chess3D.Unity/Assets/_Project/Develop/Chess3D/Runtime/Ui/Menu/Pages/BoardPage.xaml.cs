#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

using Chess3D.Runtime.Menu.UI.ViewModels;
using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
    public partial class BoardPage : UserControl
    {
        public BoardPage()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<BoardPageViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            this.Focus();
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                GameMenuBase.Instance.ChangePage<MainPage>();
                args.Handled = true;
            }
        }

        public void Back_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<MainPage>();
        }

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());
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