#if UNITY_5_3_OR_NEWER
#define NOESIS
using Chess3D.Runtime.Core.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Noesis;
using UnityEngine.Scripting;

#else
using System;
using System.Windows.Controls;
using System.Windows.Input;
#endif

namespace Chess3D.Runtime.Core.UI.UserControls
{
#if NOESIS
    [Preserve]
#endif
    public sealed partial class CorePopup : UserControl
    {
        public CorePopup()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void Popup_OnOpened(object sender, EventArgs e)
        {
            Focus();
            Keyboard.Focus(PopupNoButton);
        }

        private void OnInitialized(object sender, EventArgs e)
        {
#if NOESIS
            // Need to use ServiceLocator because this view is loaded by MainView from xaml
            DataContext = ServiceLocator.Resolve<PopupViewModel>();
#endif
        }

#if NOESIS

        private Popup Popup { get; set; }
        private Button PopupYesButton { get; set; }
        private Button PopupNoButton { get; set; }

        private void InitializeComponent()
        {
            XamlUtils.Load(this);

            Popup = FindName(nameof(Popup)) as Popup;
            PopupYesButton = FindName(nameof(PopupYesButton)) as Button;
            PopupNoButton = FindName(nameof(PopupNoButton)) as Button;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Opened" && handlerName == nameof(Popup_OnOpened))
            {
                ((Popup)source).Opened += Popup_OnOpened;
                return true;
            }

            return false;
        }

#endif
    }
}