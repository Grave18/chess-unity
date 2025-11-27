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
            TextBox.KeyDown += TextBox_KeyDown;
#if NOESIS
            DataContext = ServiceLocator.Resolve<FenTextBoxViewModel>();
#endif
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

#if NOESIS
        private TextBox TextBox { get; set; }
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, XamlUtils.GetXamlPathFromFilePath());

            TextBox = FindName("TextBox") as TextBox;
        }
#endif
    }
}
