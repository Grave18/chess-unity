#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
#endif

using Ui.Auxiliary;
using InGameMenuViewModel = Chess3D.Runtime.Core.Ui.ViewModels.InGameMenuViewModel;
using InGamePageViewModel = Chess3D.Runtime.Core.Ui.ViewModels.InGamePageViewModel;

namespace Ui.InGame.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
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
            // TODO: Refactor to use container
            DataContext = Object.FindAnyObjectByType<InGamePageViewModel>();
            // ResumeButton.DataContext = Object.FindAnyObjectByType<InGameMenuViewModel>();
#endif
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<InGameSettingsPage>();
        }

#if NOESIS
        private Button ResumeButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            ResumeButton = FindName("ResumeButton") as Button;
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