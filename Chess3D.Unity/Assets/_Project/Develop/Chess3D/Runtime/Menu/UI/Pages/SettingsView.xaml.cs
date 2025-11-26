#if UNITY_5_3_OR_NEWER
#define NOESIS
using UnityEngine.Scripting;
using Noesis;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using GUI = Noesis.GUI;
using EventArgs = Noesis.EventArgs;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

namespace Chess3D.Runtime.Menu.UI.Views
{

#if NOESIS
    [Preserve]
#endif
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            GameSettingsTab.DataContext = ServiceLocator.Resolve<GameSettingsViewModel>();
            GraphicsSettingsTab.DataContext = ServiceLocator.Resolve<GraphicsSettingsViewModel>();
            AudioSettingsTab.DataContext = ServiceLocator.Resolve<AudioSettingsViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Focus();
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
#endif
    }
}