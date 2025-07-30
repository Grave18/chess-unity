#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Ui.InGame.ViewModels;
using EventArgs = Noesis.EventArgs;
using GUI = Noesis.GUI;
using Object = UnityEngine.Object;

#else
using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
#endif

using Ui.Auxiliary;

namespace Ui.InGame
{
    public partial class InGameMenu : GameMenuBase
    {
        public InGameMenu()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();
        }

        protected override void ChangePage(UserControl page)
        {
            ContentControl.Content = page;
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<InGameMenuViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // Focus on Menu and listen for key events
            this.Focusable = true;
            this.Focus();
            this.KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                OpenOrCloseMenu();
                args.Handled = true;
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
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/InGameMenu.xaml");

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