#if UNITY_5_3_OR_NEWER
#define NOESIS
using System;
using Noesis;
using Ui.InGame.ViewModels;
using Ui.Menu.ViewModels;
using EventArgs = Noesis.EventArgs;
using GUI = Noesis.GUI;
using Object = UnityEngine.Object;

#else
using System;
using System.Windows.Controls;
#endif

namespace Ui.InGame
{
    public partial class InGameMenu : UserControl
    {
        public static InGameMenu Instance { get; private set; }

        public InGameMenu()
        {
            Instance = this;

            Initialized += OnInitialized;
            InitializeComponent();
        }

        public void ChangePage<T>() where T : UserControl
        {
            ContentControl.Content = Activator.CreateInstance(typeof(T));
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
            GUI.LoadComponent(this, "Assets/Code/Ui/InGame/InGameMenu.xaml");

            ContentControl = FindName("ContentControl") as ContentControl;
        }
#endif
    }
}