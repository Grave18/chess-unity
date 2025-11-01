#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Ui.Menu.ViewModels;
using GUI = Noesis.GUI;
using Object = UnityEngine.Object;
using EventArgs = Noesis.EventArgs;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using EventArgs = System.EventArgs;
#endif

using System.Windows.Input;
using Ui.Auxiliary;

namespace Ui.InGame.Pages
{
    public partial class InGameSettingsPage : UserControl
    {
        public InGameSettingsPage()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();

            this.KeyDown += OnKeyDown;
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<SettingsPageViewModel>();
            GraphicsSettingsTab.DataContext = Object.FindAnyObjectByType<GraphicsSettingsViewModel>();
            AudioSettingsTab.DataContext = Object.FindAnyObjectByType<AudioSettingsViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focusable = true;
            this.Focus();
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
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/Pages/InGameSettingsPage.xaml");

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