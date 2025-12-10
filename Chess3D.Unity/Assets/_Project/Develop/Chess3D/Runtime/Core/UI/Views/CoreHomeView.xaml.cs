#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using Chess3D.Runtime.Utilities;
using UnityEngine.Scripting;
using Chess3D.Runtime.Core.UI.ViewModels;

#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
#endif

namespace Chess3D.Runtime.Core.UI.Views
{
#if NOESIS
    [Preserve]
#endif
    public partial class CoreHomeView : UserControl
    {
        public CoreHomeView()
        {
            InitializeComponent();
        }

#if NOESIS
        public CoreHomeView(CoreHomeViewModel coreHomeViewModel, CoreMainViewModel coreMainViewModel) : this()
        {
            DataContext = coreHomeViewModel;
            ResumeButton.DataContext = coreMainViewModel;
        }

        private Button ResumeButton { get; set; }

        private void InitializeComponent()
        {
            XamlUtils.Load(this);

            ResumeButton = FindName("ResumeButton") as Button;
        }
#endif
    }
}