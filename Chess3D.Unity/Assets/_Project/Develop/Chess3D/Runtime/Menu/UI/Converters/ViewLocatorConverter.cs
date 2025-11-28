#if UNITY_5_3_OR_NEWER
#define NOESIS
using System;
using Chess3D.Runtime.Core;
using Chess3D.Runtime.Menu.UI.ViewModels;
using Noesis;
using UnityEngine.Scripting;

#else
using System;
using System.Windows.Data;
#endif

using System.Globalization;

namespace Ui.Converters
{
#if NOESIS
    [Preserve]
    public class ViewLocatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IViewModel viewModel && targetType == typeof(BaseComponent))
            {
                string viewTypeName = GetViewType(viewModel, out Type viewType);

                if (viewType == null)
                {
                    return new TextBlock { Text = $"Not found: {viewTypeName}" };
                }

                var instance = ServiceLocator.Resolve(viewType);

                return instance;
            }

            return null;
        }

        private static string GetViewType(IViewModel viewModel, out Type viewType)
        {
            Type vmType = viewModel.GetType();
            string viewTypeName = vmType.FullName?.Replace("ViewModel", "View");
            viewType = Type.GetType(viewTypeName ?? string.Empty);
            return viewTypeName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

#else
    public class ViewLocatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
#endif
}