#if UNITY_5_3_OR_NEWER
using Noesis;
using UnityEngine.Scripting;
#endif

namespace Chess3D.Runtime.Core.UI
{
#if UNITY_5_3_OR_NEWER
    [Preserve]
#endif
    public static class Attached
    {
        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.RegisterAttached("Show", typeof(bool), typeof(Attached), new PropertyMetadata(false));

        public static bool GetShow(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowProperty);
        }

        public static void SetShow(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowProperty, value);
        }
    }
}