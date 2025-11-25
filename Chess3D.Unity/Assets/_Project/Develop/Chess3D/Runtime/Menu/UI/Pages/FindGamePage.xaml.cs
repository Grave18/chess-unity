#if UNITY_5_3_OR_NEWER
#define NOESIS
using UnityEngine.Scripting;
using Noesis;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
#if NOESIS
    [Preserve]
#endif
    public partial class FindGamePage : UserControl
    {
        public FindGamePage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = ServiceLocator.Resolve<FindGameViewModel>();
#endif
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            GameMenuBase.Instance.ChangePage<PlayPage>("OnlineTab");
        }

#if NOESIS

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());
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