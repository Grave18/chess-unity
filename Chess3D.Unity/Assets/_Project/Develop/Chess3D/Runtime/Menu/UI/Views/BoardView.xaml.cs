#if UNITY_5_3_OR_NEWER
#define NOESIS
using Chess3D.Runtime.Menu.UI.ViewModels;
using UnityEngine.Scripting;
using Noesis;
using Chess3D.Runtime.Utilities;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

namespace Chess3D.Runtime.Menu.UI.Views
{
#if NOESIS
    [Preserve]
#endif
    public partial class BoardView : UserControl
    {
        public BoardView()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

#if NOESIS
        public BoardView(BoardViewModel boardViewModel) : this()
        {
            DataContext = boardViewModel;
        }
#endif

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Focus();
        }

#if NOESIS

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());
        }
#endif
    }
}