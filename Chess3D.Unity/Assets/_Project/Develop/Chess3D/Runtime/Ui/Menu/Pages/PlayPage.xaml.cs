#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Ui.Menu.ViewModels;
using UnityEngine;
using Grid = Noesis.Grid;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

using Ui.Auxiliary;

namespace Ui.Menu.Pages
{
    public partial class PlayPage : UserControl
    {
        private readonly string _selectedTab;
        public PlayPage()
        {
            Initialized += OnInitialized;
            Loaded += OnLoaded;
            InitializeComponent();
        }

        public PlayPage(string selectedTab) : this()
        {
            _selectedTab = selectedTab;
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            this.DataContext = Object.FindAnyObjectByType<PlayPageViewModel>();
            Root.DataContext = Object.FindAnyObjectByType<GameSettingsViewModel>();
#endif
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            SelectTab();
            SetupInputPlayPage();
        }

        private void SelectTab()
        {
            if (_selectedTab != null)
            {
                foreach (TabItem tabItem in TabControl.Items)
                {
                    if (tabItem.Name == _selectedTab)
                    {
                        TabControl.SelectedItem = tabItem;
                        return;
                    }
                }

                LogUi.Debug($"Tab {_selectedTab} not found");
            }
        }

        private void SetupInputPlayPage()
        {
            TabControl.Focus();
            this.KeyDown += PlayPage_OnKeyDown;
        }

        private void PlayPage_OnKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                GameMenuBase.Instance.ChangePage<MainPage>();
                args.Handled = true;
            }
        }

        public void Back_Click(object sender, RoutedEventArgs args)
        {
            GameMenuBase.Instance.ChangePage<MainPage>();
        }

        private void CreateLobby_OnClick(object sender, RoutedEventArgs e)
        {
            CreateLobbyButton.Command.Execute(null);
            GameMenuBase.Instance.ChangePage<LobbyPage>();
        }

        private void FindGame_OnClick(object sender, RoutedEventArgs e)
        {
            FindGameButton.Command.Execute(null);
            GameMenuBase.Instance.ChangePage<FindGamePage>();
        }

#if NOESIS
        private Grid Root { get; set; }

        private TabControl TabControl { get; set; }
        private TabItem OnlineTab { get; set; }
        private TabItem OfflineTab { get; set; }
        private TabItem ComputerTab { get; set; }

        private Button CreateLobbyButton { get; set; }
        private Button FindGameButton { get; set; }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            Root = FindName("Root") as Grid;

            TabControl = FindName("TabControl") as TabControl;
            OnlineTab = FindName("OnlineTab") as TabItem;
            OfflineTab = FindName("OfflineTab") as TabItem;
            ComputerTab = FindName("ComputerTab") as TabItem;

            CreateLobbyButton = FindName("CreateLobbyButton") as Button;
            FindGameButton = FindName("FindGameButton") as Button;
        }

        protected override bool ConnectEvent(object source, string eventName, string handlerName)
        {
            if (eventName == "Click" && handlerName == nameof(Back_Click))
            {
                ((Button)source).Click += Back_Click;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(CreateLobby_OnClick))
            {
                ((Button)source).Click += CreateLobby_OnClick;
                return true;
            }

            if (eventName == "Click" && handlerName == nameof(FindGame_OnClick))
            {
                ((Button)source).Click += FindGame_OnClick;
                return true;
            }

            return false;
        }
#endif
    }
}