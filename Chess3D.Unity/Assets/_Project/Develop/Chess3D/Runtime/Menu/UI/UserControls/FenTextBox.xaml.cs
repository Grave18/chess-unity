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
using System.Windows.Data;
using System.Windows.Input;
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
#if NOESIS
            Initialized += OnInitialized;
#endif
            InitializeComponent();
        }
#if NOESIS

        private void OnInitialized(object sender, EventArgs args)
        {
            TextBox.KeyDown += TextBox_KeyDown;
            DataContext = ServiceLocator.Resolve<FenTextBoxViewModel>();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                var textBox = (TextBox)sender;
                BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                bindingExpression?.UpdateSource();

                MoveFocusToParent(textBox);
            }
        }

        private static void MoveFocusToParent(FrameworkElement element)
        {
            FrameworkElement parent = element.Parent?.Parent;
            if (parent != null)
            {
                parent.Focus();
            }
        }

        private TextBox TextBox { get; set; }
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            TextBox = FindName("TextBox") as TextBox;
        }
#endif
    }
}
