#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine.Scripting;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using EventArgs = Noesis.EventArgs;
using GUI = Noesis.GUI;
using Chess3D.Runtime.Utilities;

#else
using System;
using System.Windows.Controls;
#endif

using Ui.Auxiliary;

namespace Ui.Menu
{

#if NOESIS
    [Preserve]
#endif
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

        private async void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = await ServiceLocator.ResolveAsync<MainMenuViewModel>();
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