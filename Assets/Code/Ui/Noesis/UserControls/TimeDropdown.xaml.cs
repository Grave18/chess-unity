#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;

#else
using System.Windows.Controls;
#endif

namespace Ui.Noesis.UserControls
{
    /// <summary>
    /// Interaction logic for TimeDropdown.xaml
    /// </summary>
    public partial class TimeDropdown : UserControl
    {
        public TimeDropdown()
        {
            InitializeComponent();
        }

        #if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Noesis/UserControls/TimeDropdown.xaml");
        }
        #endif
    }
}
