using System;
using UnityEngine;

namespace Chess3D.Runtime.UtilsCommon.ObjectCreation
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
                return CreateWithoutParameters<T>(e);
            }

            return instance;
        }

        private static T CreateWithoutParameters<T>(Exception e)
        {
#if UNITY_5_3_OR_NEWER
            Debug.LogException(e);
#endif
            try
            {
                var instance = (T)Activator.CreateInstance(typeof(T));
                return instance;
            }
            catch (Exception e2)
            {
#if UNITY_5_3_OR_NEWER
                Debug.LogException(e2);
#endif
                return default;
            }
        }
    }
}