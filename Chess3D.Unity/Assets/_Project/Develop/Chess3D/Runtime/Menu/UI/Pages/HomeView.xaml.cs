#if UNITY_5_3_OR_NEWER
#define NOESIS
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Noesis;
using UnityEngine.Scripting;
using GUI = Noesis.GUI;

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
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();
        }

#if NOESIS
        public HomeView(HomeViewModel homeViewModel) : this()
        {
            DataContext = homeViewModel;
        }
#endif

        private void OnInitialized(object sender, EventArgs e)
        {
            SetupPopup();
        }

        private void SetupPopup()
        {
            PopupText.Text = "Are you sure you want to Exit?";
            PopupYesButton.Content = "Exit";
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            KeyDown += HomeView_OnKeyDown;
            PlayButton.Focus();
        }

        private void HomeView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (Popup.IsOpen)
                {
                    ClosePopup();
                }
                else
                {
                    OpenPopup();
                }
                e.Handled = true;
            }

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OpenPopup();
        }

        private void ExitButton_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                OpenPopup();
            }
        }

        private void OpenPopup()
        {
            Popup.IsOpen = true;
            PopupCancelButton.Focus();
        }

        private void PopupClose_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            Popup.IsOpen = false;
            ExitButton.Focus();
        }

#if NOESIS
        private Popup Popup { get; set; }
        private TextBlock PopupText { get; set; }
        private Button PopupYesButton { get; set; }
        private Button PopupCancelButton { get; set; }
        private Button ExitButton { get; set; }
        private Button PlayButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            Popup = FindName("Popup") as Popup;
            PopupText = FindName("PopupText") as TextBlock;
            PopupYesButton = FindName("PopupYesButton") as Button;
            PopupCancelButton = FindName("PopupCancelButton") as Button;
            ExitButton = FindName("ExitButton") as Button;
            PlayButton = FindName("PlayButton") as Button;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Exit_Click))
            {
                ((Button)source).Click += Exit_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(PopupClose_Click))
            {
                ((Button)source).Click += PopupClose_Click;
                return true;
            }

            if (eventName == "KeyDown" && handlerName == nameof(ExitButton_OnKeyDown))
            {
                ((UIElement)source).KeyDown += ExitButton_OnKeyDown;
                return true;
            }

            return false;
        }
#endif
    }
}