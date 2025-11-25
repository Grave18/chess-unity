#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine.Scripting;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Chess3D.Runtime.Utilities;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows.Controls;
#endif

namespace Ui.Menu.UserControls
{
#if NOESIS
    [Preserve]
#endif
    public partial class FenTextBox : UserControl
    {
        public FenTextBox()
        {
            Initialized += OnInitialized;
            InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
#if NOESIS
            DataContext = ServiceLocator.Resolve<FenUserControlViewModel>();
#endif
        }

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());
        }
#endif
    }
}
