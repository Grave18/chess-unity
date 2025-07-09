#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;
using Ui.InGame.ViewModels;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
#endif

using Ui.Auxiliary;

namespace Ui.InGame.Pages
{
    public partial class InGamePage : UserControl
    {
        public InGamePage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<InGamePageViewModel>();
            ResumeButton.DataContext = Object.FindAnyObjectByType<InGameMenuViewModel>();
#endif

            // Add to InGame Page ability to handle keyboard Esc and Enter
            this.KeyUp += OnKeyUp;
        }

        /// Handle popup buttons handling by code
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if(!Popup.IsOpen)
            {
                return;
            }

            if (e.Key == Key.Escape)
            {
                PopupClose_Click(null, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                PopupButton.Command.Execute(null);
                PopupYes_Click(null, null);
                e.Handled = true;
            }
        }

        private void PopupClose_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void PopupYes_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
            CloseMenu();
        }

        private void ClosePopup()
        {
            Popup.IsOpen = false;
        }

        private void CloseMenu()
        {
            if (FindName("Border") is UIElement border)
            {
                border.Visibility = Visibility.Collapsed;
            }
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<InGameSettingsPage>();
        }

        private void Resign_Click(object sender, RoutedEventArgs e)
        {
            SetupPopup("Are you sure you want to Resign?", "Resign", "ResignCommand");
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            SetupPopup("Are you sure you want to Draw?", "Draw", "DrawCommand");
        }

        private void Rematch_Click(object sender, RoutedEventArgs e)
        {
            SetupPopup("Are you sure you want to Rematch?", "Rematch", "RematchCommand");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SetupPopup("Are you sure you want to Exit?", "Exit", "ExitToMainMenuCommand");
        }

        private void SetupPopup(string header, string buttonText, string commandName)
        {
            Popup.IsOpen = true;
            PopupText.Text = header;

            var binding = new Binding(commandName);
            PopupButton.SetBinding(ButtonBase.CommandProperty, binding);
            PopupButton.Content = buttonText;
        }

#if NOESIS

        private Popup Popup { get; set; }
        private Button PopupButton { get; set; }
        private TextBlock PopupText { get; set; }

        private Button ResumeButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/Pages/InGamePage.xaml");

            ResumeButton = FindName("ResumeButton") as Button;

            Popup = FindName("Popup") as Popup;
            PopupButton = FindName("PopupButton") as Button;
            PopupText = FindName("PopupText") as TextBlock;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Settings_Click))
            {
                ((Button)source).Click += Settings_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Resign_Click))
            {
                ((Button)source).Click += Resign_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Draw_Click))
            {
                ((Button)source).Click += Draw_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(Rematch_Click))
            {
                ((Button)source).Click += Rematch_Click;
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

            if (eventName == "Click" && handlerName == nameof(Exit_Click))
            {
                ((Button)source).Click += Exit_Click;
                return true;
            }

            return false;
        }

#endif
    }
}