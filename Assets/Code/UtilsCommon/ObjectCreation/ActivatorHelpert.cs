using System;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace UtilsCommon.ObjectCreation
{
    public static class ActivatorHelper
    {
        public static T CreateInstance<T>(params object[] args)
        {
            T instance;

            try
            {
                instance = (T)Activator.CreateInstance(typeof(T), args);
            }
            catch (Exception e)
            {
#if UNITY_5_3_OR_NEWER
                Debug.LogException(e);
#endif
                return Activator.CreateInstance<T>();
            }

            return instance;
        }
    }
}