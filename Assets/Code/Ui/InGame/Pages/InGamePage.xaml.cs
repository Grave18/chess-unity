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
        private CheckBox _saveCheckBox;

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

            CreateSaveCheckBox();
        }

        /// Handle popup buttons handling by code
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!Popup.IsOpen)
            {
                return;
            }

            if (e.Key == Key.Escape)
            {
                PopupNoButton.Command.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                PopupYesButton.Command.Execute(null);
                e.Handled = true;
            }
        }

        private void CreateSaveCheckBox()
        {
            const string styleName = "CheckboxStyle";
#if NOESIS
            var style = TryFindResource(styleName) as Style;

            if (style == null)
            {
                LogUi.Debug($"CheckBoxStyle not found");
            }
#else
            ResourceDictionary appResources = Application.Current.Resources;

            if (!appResources.Contains(styleName))
            {
                return;
            }

            var style = appResources[styleName] as Style;
#endif

            _saveCheckBox = new CheckBox
            {
                IsChecked = false,
                Content = "Do you want to save the game?",
                Style = style
            };

            var binding = new Binding("IsSaveBoard")
            {
                Mode = BindingMode.TwoWay
            };

            _saveCheckBox.SetBinding(ToggleButton.IsCheckedProperty, binding);
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<InGameSettingsPage>();
        }

#if NOESIS

        private Popup Popup { get; set; }
        private DockPanel PopupDockPanel { get; set; }
        private Button PopupYesButton { get; set; }
        private Button PopupNoButton { get; set; }
        private TextBlock PopupText { get; set; }
        private Button ResumeButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/Pages/InGamePage.xaml");

            ResumeButton = FindName("ResumeButton") as Button;
            Popup = FindName("Popup") as Popup;
            PopupDockPanel = FindName("PopupDockPanel") as DockPanel;
            PopupYesButton = FindName("PopupYesButton") as Button;
            PopupNoButton = FindName("PopupNoButton") as Button;
            PopupText = FindName("PopupText") as TextBlock;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Settings_Click))
            {
                ((Button)source).Click += Settings_Click;
                return true;
            }

            return false;
        }

#endif
    }
}