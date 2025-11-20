#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using EventArgs = Noesis.EventArgs;
using GUI = Noesis.GUI;
using Object = UnityEngine.Object;

#else
using System;
using System.Windows.Controls;
#endif

using Chess3D.Runtime.Menu.UI.ViewModels;
using Ui.Auxiliary;

namespace Ui.Menu
{
    public partial class MainMenu : GameMenuBase
    {
        public MainMenu()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        protected override void ChangePage(UserControl page)
        {
            ContentControl.Content = page;
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = Object.FindAnyObjectByType<MainMenuViewModel>();
#endif
        }

#if NOESIS
        private ContentControl ContentControl { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            ContentControl = FindName("ContentControl") as ContentControl;
        }
#endif
    }
}