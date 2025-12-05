#if UNITY_5_3_OR_NEWER
#define NOESIS
using UnityEngine.Scripting;
using Noesis;
using Chess3D.Runtime.Utilities;
using Chess3D.Runtime.Menu.UI.ViewModels;

#else
using System;
using System.Windows.Controls;
using System.Windows;
#endif

namespace Chess3D.Runtime.Menu.UI.Views
{
#if NOESIS
    [Preserve]
#endif
    public partial class LobbyView : UserControl
    {
        public LobbyView()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

#if NOESIS
        private LobbyView(LobbyViewModel lobbyViewModel) : this()
        {
            DataContext = lobbyViewModel;
        }

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());
        }
#endif
    }
}