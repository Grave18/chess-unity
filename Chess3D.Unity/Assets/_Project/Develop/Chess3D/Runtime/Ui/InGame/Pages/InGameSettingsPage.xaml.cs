#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine.Scripting;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using GUI = Noesis.GUI;
using Object = UnityEngine.Object;
using EventArgs = Noesis.EventArgs;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EventArgs = System.EventArgs;
#endif

using Ui.Auxiliary;

namespace Ui.InGame.Pages
{
#if NOESIS
    [Preserve]
#endif
    public partial class InGameSettingsPage : UserControl
    {
        public InGameSettingsPage()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();

            KeyDown += OnKeyDown;
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = ServiceLocator.Resolve<SettingsPageViewModel>();
            GraphicsSettingsTab.DataContext = ServiceLocator.Resolve<GraphicsSettingsViewModel>();
            AudioSettingsTab.DataContext = ServiceLocator.Resolve<AudioSettingsViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Focusable = true;
            Focus();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Back to main page
            if (e.Key == Key.Escape)
            {
                Back_Click();
                e.Handled = true;
            }
        }

        public void Back_Click(object sender = null, RoutedEventArgs args = null)
        {
            GameMenuBase.Instance.ChangePage<InGamePage>();
        }

#if NOESIS
        private TabItem GraphicsSettingsTab { get; set; }
        private TabItem AudioSettingsTab { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

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