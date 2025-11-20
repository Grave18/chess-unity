#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

using Chess3D.Runtime.Menu.UI.ViewModels;
using Ui.Auxiliary;
using EventArgs = Noesis.EventArgs;
using Object = UnityEngine.Object;

namespace Ui.Menu.Pages
{
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            #if NOESIS
                DataContext = Object.FindAnyObjectByType<SettingsPageViewModel>();
                GameSettingsTab.DataContext = Object.FindAnyObjectByType<GameSettingsViewModel>();
                GraphicsSettingsTab.DataContext = Object.FindAnyObjectByType<GraphicsSettingsViewModel>();
                AudioSettingsTab.DataContext = Object.FindAnyObjectByType<AudioSettingsViewModel>();
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
        private TabItem GameSettingsTab { get; set; }
        private TabItem GraphicsSettingsTab { get; set; }
        private TabItem AudioSettingsTab { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            GameSettingsTab = FindName("GameSettingsTab") as TabItem;
            GraphicsSettingsTab = FindName("GraphicsSettingsTab") as TabItem;
            AudioSettingsTab = FindName("AudioSettingsTab") as TabItem;
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