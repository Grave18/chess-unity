#if UNITY_5_3_OR_NEWER
#define NOESIS
using UnityEngine.Scripting;
using Noesis;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;

#else
using System;
using System.Windows.Controls;
using System.Windows;
#endif

using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
#if NOESIS
    [Preserve]
#endif
    public partial class LobbyPage : UserControl
    {
        public LobbyPage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = ServiceLocator.Resolve<LobbyViewModel>();
#endif
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Back.Command.Execute(null); // for command to execute before event handler
            // GameMenuBase.Instance.ChangePage<PlayView>("OnlineTab");
        }

#if NOESIS

        private Button Back { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            Back = FindName("Back") as Button;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Back_Click))
            {
                ((Button)source).Click += Back_Click;
                return true;
            }

            return false;
        }

#endif
    }
}