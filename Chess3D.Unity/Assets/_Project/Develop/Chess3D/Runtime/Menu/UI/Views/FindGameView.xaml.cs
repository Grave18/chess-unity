#if UNITY_5_3_OR_NEWER
#define NOESIS
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using Noesis;
using UnityEngine.Scripting;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

namespace Chess3D.Runtime.Menu.UI.Views
{
#if NOESIS
    [Preserve]
#endif
    public partial class FindGameView : UserControl
    {
        public FindGameView()
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
            // GameMenuBase.Instance.ChangePage<PlayView>("OnlineTab");
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