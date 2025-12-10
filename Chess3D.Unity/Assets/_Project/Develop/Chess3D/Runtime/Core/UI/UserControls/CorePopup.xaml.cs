#if UNITY_5_3_OR_NEWER
#define NOESIS
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Utilities;
using Noesis;
using GUI = Noesis.GUI;
using PopupViewModel = Chess3D.Runtime.Core.UI.ViewModels.PopupViewModel;

#else
using System;
using System.Windows.Controls;
using System.Windows.Input;
#endif

namespace Chess3D.Runtime.Core.UI.UserControls
{
    public partial class CorePopup : UserControl
    {
        public CorePopup()
        {
            Initialized += OnInitialized;
            InitializeComponent();

            KeyDown += OnKeyDown;
        }

        private void OnInitialized(object sender, EventArgs e)
        {
#if NOESIS
            DataContext = ServiceLocator.Resolve<PopupViewModel>();
#endif
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Popup.IsOpen)
            {
                if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                    PopupNoButton.Command.Execute(null);
                }
                else if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    PopupYesButton.Command.Execute(null);
                }
            }
        }

        private void Popup_OnOpened(object sender, EventArgs e)
        {
            Focusable = true;
            Focus();
        }

#if NOESIS

        private Popup Popup { get; set; }
        private Button PopupYesButton { get; set; }
        private Button PopupNoButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

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