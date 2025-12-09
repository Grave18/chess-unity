#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Utilities;
using UnityEngine.Scripting;
using EventArgs = Noesis.EventArgs;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
#endif

using Chess3D.Runtime.Menu.UI.Auxiliary;
using InGameMenuViewModel = Chess3D.Runtime.Core.UI.ViewModels.InGameMenuViewModel;

namespace Ui.InGame
{
#if NOESIS
    [Preserve]
#endif
    public partial class InGameMenu : GameMenuBase
    {
        public InGameMenu()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();
            KeyDown += OnKeyDown;
        }

        protected override void ChangePage(UserControl page)
        {
            ContentControl.Content = page;
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = ServiceLocator.Resolve<InGameMenuViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // Focus on Menu and listen for key events
            Focusable = true;
            Focus();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OpenOrCloseMenu();
                e.Handled = true;
            }
        }

        private void OpenOrCloseMenu()
        {
            SandwichButton.Command.Execute(null);
        }

#if NOESIS

        private ContentControl ContentControl { get; set; }
        private UIElement SlidingPanel { get; set; }
        private Button SandwichButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            ContentControl = FindName("ContentControl") as ContentControl;
            SlidingPanel = FindName("SlidingPanel") as UIElement;
            SandwichButton = FindName("SandwichButton") as Button;
        }

#endif
    }

    public static class Attached
    {
        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.RegisterAttached("Show", typeof(bool), typeof(Attached), new PropertyMetadata(false));

        public static bool GetShow(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowProperty);
        }

        public static void SetShow(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowProperty, value);
        }
    }
}