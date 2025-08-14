#if UNITY_5_3_OR_NEWER
#define NOESIS
using UnityEngine;
using Noesis;
using Ui.Menu.ViewModels;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
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
            DataContext = Object.FindAnyObjectByType<FindGameViewModel>();
#endif
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            GameMenuBase.Instance.ChangePage<PlayPage>("OnlineTab");
        }

#if NOESIS

        private void InitializeComponent()
        {
            Noesis.GUI.LoadComponent(this, "Assets/Code/Ui/Menu/Pages/FindGamePage.xaml");
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