#if UNITY_5_3_OR_NEWER
#define NOESIS
using Chess3D.Runtime.Utilities;
using Noesis;
using UnityEngine.Scripting;
using GUI = Noesis.GUI;

#else
using System.Windows.Controls;
#endif

namespace Chess3D.Runtime.Menu.UI.UserControls
{
#if NOESIS
    [Preserve]
#endif
    public sealed partial class DifficultyDropdown : UserControl
    {
        public DifficultyDropdown()
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
