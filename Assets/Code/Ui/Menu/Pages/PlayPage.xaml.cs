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
using System.Windows.Input;
#endif

using System.Collections.Generic;
using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
    public partial class PlayPage : UserControl
    {
        public PlayPage()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<PlayPageViewModel>();
            Root.DataContext = Object.FindAnyObjectByType<GameSettingsViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            SetupInputPlayPage();
            SetupInputTabControl();
        }

        private void SetupInputPlayPage()
        {
            TabControl.Focus();
            this.KeyDown += PlayPage_OnKeyDown;
        }

        private void SetupInputTabControl()
        {
            // AddHandler(PreviewKeyDownEvent, new KeyEventHandler(TabControl_PreviewKeyDown));
            TabControl.KeyDown += TabControl_PreviewKeyDown;
        }

        private void PlayPage_OnKeyDown(object sender, KeyEventArgs args)
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

        private void TabControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Only handle arrow navigation
            if (Keyboard.FocusedElement is { } focusedElement)
            {
                if (e.Key == Key.Down)
                {
                    // Try to move focus down
                    var request = new TraversalRequest(FocusNavigationDirection.Down);
                    if (focusedElement.MoveFocus(request))
                    {
                        e.Handled = true;
                    }
                }
                else if (e.Key == Key.Up)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Up);
                    if (focusedElement.MoveFocus(request))
                    {
                        e.Handled = true;
                    }
                }
                else if (e.Key is Key.Left or Key.Right)
                {
                    // Let TabControl handle Left/Right for tab switching
                    e.Handled = false;
                }
            }
        }

#if NOESIS
        public Grid Root { get; set; }
        private TabControl TabControl { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/Pages/PlayPage.xaml");

            Root = FindName("Root") as Grid;
            TabControl = FindName("TabControl") as TabControl;
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