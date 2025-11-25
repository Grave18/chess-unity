#if UNITY_5_3_OR_NEWER
#define NOESIS
using UnityEngine.Scripting;
using Noesis;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
#if NOESIS
    [Preserve]
#endif
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
            DataContext = ServiceLocator.Resolve<BoardPageViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Focus();
            KeyDown += OnKeyDown;
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