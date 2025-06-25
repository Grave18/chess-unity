#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;

#else
using Blend.DesignTimeViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
#endif

namespace Ui.Noesis
{
    public partial class VsPlayerPage : UserControl
    {
        public VsPlayerPage()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

#if NOESIS
        private ComboBox ComboBox { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Noesis/VsPlayerPage.xaml");

            ComboBox = FindName("ComboBox") as ComboBox;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Back_Click))
            {
                ((Button)source).Click += Back_Click;
                return true;
            }

            if (eventName == "SelectionChanged" && handlerName == nameof(ComboBox_SelectionChanged))
            {
                ((ComboBox)source).SelectionChanged += ComboBox_SelectionChanged;
                return true;
            }

            return false;
        }
#endif

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<VsPlayerPageViewModel>();
#else
            DataContext = new VsPlayerPageViewModelDesign();
#endif
        }

        public void Back_Click(object sender, RoutedEventArgs args)
        {
            MainMenu.Instance.ChangePage<MainPage>();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Empty
        }
    }
}