using System;
using System.Globalization;

#if UNITY_5_3_OR_NEWER
using Noesis;

#else
using System.Windows.Data;
#endif

namespace Chess3D.Runtime.Menu.UI.Converters
{
    public sealed class FriendFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
 #if UNITY_5_3_OR_NEWER
            if (value is int friendFilter && targetType == typeof(BaseComponent))
            {
                return friendFilter switch
                {
                    0 => "In Game",
                    1 => "Online",
                    2 => "All",
                    _ => "Unknown"
                };
            }
  #endif
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}