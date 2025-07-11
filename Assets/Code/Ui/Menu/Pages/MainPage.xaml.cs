#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using GUI = Noesis.GUI;
using UnityEngine; // Do not remove

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

using System.Windows.Input;
using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            SetupKeyBindingToClosePopup();
        }

        private void SetupKeyBindingToClosePopup()
        {
            this.Focusable = true;
            this.Focus();
            this.KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!Popup.IsOpen)
            {
                return;
            }

            if (e.Key == Key.Escape)
            {
                ClosePopup();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Exit();
            }
        }

        public void Play_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<PlayPage>();
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<SettingsPage>();
        }


        private void Board_Click(object sender, RoutedEventArgs e)
        {
            GameMenuBase.Instance.ChangePage<BoardPage>();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OpenPopup();
            SetupPopup();
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
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
        }

        private void SetupPopup()
        {
            PopupText.Text = "Are you sure you want to Exit?";
            PopupYesButton.Content = "Exit";
            PopupYesButton.Click += PopupYes_Click;
        }

        private void PopupClose_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            Popup.IsOpen = false;
        }

        private void PopupYes_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif NOESIS
            Application.Quit();
#else
            Application.Current.Shutdown();
#endif
        }

#if NOESIS
        private Popup Popup { get; set; }
        private TextBlock PopupText { get; set; }
        private Button PopupYesButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/Pages/MainPage.xaml");

            Popup = FindName("Popup") as Popup;
            PopupText = FindName("PopupText") as TextBlock;
            PopupYesButton = FindName("PopupYesButton") as Button;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Play_Click))
            {
                ((Button)source).Click += Play_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Settings_Click))
            {
                ((Button)source).Click += Settings_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Board_Click))
            {
                ((Button)source).Click += Board_Click;
                return true;
            }

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

            if (eventName == "Click" && handlerName == nameof(PopupYes_Click))
            {
                ((Button)source).Click += PopupYes_Click;
                return true;
            }

            if (eventName == "KeyUp" && handlerName == nameof(UIElement_OnKeyUp))
            {
                ((UIElement)source).KeyUp += UIElement_OnKeyUp;
                return true;
            }

            return false;
        }
#endif
    }
}