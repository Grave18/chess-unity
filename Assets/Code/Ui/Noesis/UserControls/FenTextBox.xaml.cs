#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;

#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

namespace Ui.Noesis.UserControls
{
    public partial class FenTextBox : UserControl
    {
        public FenTextBox()
        {
            InitializeComponent();
        }

        #if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Code/Ui/Noesis/UserControls/FenTextBox.xaml");
        }
        #endif
    }
}
