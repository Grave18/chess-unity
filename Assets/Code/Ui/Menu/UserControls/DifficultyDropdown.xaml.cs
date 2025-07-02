#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using GUI = Noesis.GUI;

#else
using System.Windows.Controls;
#endif

namespace Ui.Menu.UserControls
{
    public partial class DifficultyDropdown : UserControl
    {
        public DifficultyDropdown()
        {
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Menu/UserControls/DifficultyDropdown.xaml");
        }
#endif
    }
}
