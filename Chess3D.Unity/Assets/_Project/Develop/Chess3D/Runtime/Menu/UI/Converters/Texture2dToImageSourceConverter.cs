#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
using UnityEngine;

#else
using System.Windows.Data;
#endif

using System;
using System.Globalization;

namespace Ui.Converters
{
    public class Texture2dToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if NOESIS
        if (value is Texture2D texture && targetType == typeof(ImageSource))
        {
            // Wrap Unity texture in Noesis TextureSource
            return new TextureSource(texture);
        }
#endif

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}