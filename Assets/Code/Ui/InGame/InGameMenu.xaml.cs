#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Ui.InGame.ViewModels;
using EventArgs = Noesis.EventArgs;
using GUI = Noesis.GUI;
using Object = UnityEngine.Object;

#else
using System;
using System.Windows.Controls;
#endif

using Ui.Auxiliary;

namespace Ui.InGame
{
    public partial class InGameMenu : GameMenuBase
    {
        public InGameMenu()
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
            DataContext = Object.FindAnyObjectByType<InGameMenuViewModel>();
#endif
        }

#if NOESIS
        private ContentControl ContentControl { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/InGameGameMenuBase.xaml");

            ContentControl = FindName("ContentControl") as ContentControl;
        }
#endif
    }
}