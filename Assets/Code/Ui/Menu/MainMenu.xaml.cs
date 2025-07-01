#if UNITY_5_3_OR_NEWER
#define NOESIS
using System;
using Noesis;
using UnityEngine;
using EventArgs = Noesis.EventArgs;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows.Controls;
#endif

namespace Ui.Menu
{
    public partial class MainMenu : UserControl
    {
        public static MainMenu Instance { get; private set; }

        public MainMenu()
        {
            Instance = this;

            Initialized += OnInitialized;
            InitializeComponent();
        }

#if NOESIS
        private ContentControl ContentControl { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/MainMenu.xaml");

            ContentControl = FindName("ContentControl") as ContentControl;
        }
#endif

        public void ChangePage<T>() where T : UserControl
        {
            ContentControl.Content = Activator.CreateInstance(typeof(T));
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            // this.DataContext = new ViewModel();
        }
    }
}