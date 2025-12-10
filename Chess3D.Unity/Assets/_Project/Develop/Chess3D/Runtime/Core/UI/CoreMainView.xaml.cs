#if UNITY_5_3_OR_NEWER
#define NOESIS
using Chess3D.Runtime.Utilities;
using Noesis;
using UnityEngine.Scripting;

#else
using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
#endif


namespace Chess3D.Runtime.Core.UI
{
#if NOESIS
    [Preserve]
#endif
    public partial class CoreMainView : UserControl
    {
        public CoreMainView()
        {
            Loaded += (_, _) => Focus();
            InitializeComponent();
        }

#if NOESIS
        private void InitializeComponent()
        {
            XamlUtils.Load(this);
        }
#endif
    }
}