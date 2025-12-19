#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using UnityEngine.Scripting;
using Grid = Noesis.Grid;
using GUI = Noesis.GUI;

#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace Chess3D.Runtime.Menu.UI.Views
{

#if NOESIS
    [Preserve]
#endif
    public sealed partial class PlayView : UserControl
    {
        private readonly string _selectedTab;
        public PlayView()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        public PlayView(string selectedTab) : this()
        {
            _selectedTab = selectedTab;
        }

#if NOESIS
        public PlayView(PlayViewModel playViewModel, GameSettingsViewModel gameSettingsViewModel) : this()
        {
            DataContext = playViewModel;
            Root.DataContext = gameSettingsViewModel;
        }
#endif

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            SelectTab();
            TabControl.Focus();
        }

        private void SelectTab()
        {
            if (_selectedTab == null)
            {
                return;
            }

            foreach (TabItem tabItem in TabControl.Items)
            {
                if (tabItem.Name == _selectedTab)
                {
                    TabControl.SelectedItem = tabItem;
                    return;
                }
            }
        }

#if NOESIS
        private Grid Root { get; set; }
        private TabControl TabControl { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            Root = FindName("Root") as Grid;
            TabControl = FindName("TabControl") as TabControl;
        }
#endif
    }
}