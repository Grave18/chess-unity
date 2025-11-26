using System;
using System.Globalization;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Noesis;
using UnityEngine.Scripting;

namespace Ui.Converters
{
    [Preserve]
    public class ViewLocatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IViewModel viewModel && targetType == typeof(BaseComponent))
            {
                Type vmType = viewModel.GetType();
                string viewTypeName = vmType.FullName?.Replace("ViewModel", "View");
                var viewType = Type.GetType(viewTypeName ?? string.Empty);

                if (viewType == null)
                {
                    return new TextBlock { Text = $"{nameof(ViewLocatorConverter)}: Not found: {viewTypeName}" };
                }

                var instance = Activator.CreateInstance(viewType) as Control;

                if (instance != null)
                {
                    instance.DataContext = viewModel;
                }

                return instance;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}