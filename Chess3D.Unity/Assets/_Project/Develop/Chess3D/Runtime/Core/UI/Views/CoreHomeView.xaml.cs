#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using GUI = Noesis.GUI;
using Chess3D.Runtime.Utilities;
using UnityEngine.Scripting;
using Chess3D.Runtime.Core.UI.ViewModels;
using Chess3D.Runtime.Menu.UI.Auxiliary;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
#endif

namespace Chess3D.Runtime.Core.UI.Views
{
#if NOESIS
    [Preserve]
#endif
    public partial class CoreHomeView : UserControl
    {
        private CheckBox _saveCheckBox;

        public CoreHomeView()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = ServiceLocator.Resolve<CoreHomeViewModel>();
            ResumeButton.DataContext = ServiceLocator.Resolve<CoreMainViewModel>();
#endif
        }

        public void Settings_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<CoreSettingsPage>();
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