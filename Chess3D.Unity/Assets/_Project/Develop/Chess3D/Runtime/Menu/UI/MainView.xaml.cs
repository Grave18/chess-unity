#if UNITY_5_3_OR_NEWER
#define NOESIS
using Chess3D.Runtime.Utilities;
using Noesis;
using UnityEngine.Scripting;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows.Controls;
#endif

namespace Chess3D.Runtime.Menu.UI
{
#if NOESIS
    [Preserve]
#endif
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());
        }
#endif
    }
}