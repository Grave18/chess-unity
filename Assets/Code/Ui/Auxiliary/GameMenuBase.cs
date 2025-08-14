#if UNITY_5_3_OR_NEWER
#define NOESIS
using UnityEngine;
using Noesis;

#else
using System;
using System.Windows.Controls;
#endif

using UtilsCommon.ObjectCreation;

namespace Ui.Auxiliary
{
    public abstract partial class GameMenuBase : UserControl
    {
        public static GameMenuBase Instance { get; private set; }

        public void ChangePage<T>(params object[] args) where T : UserControl
        {
            var newPage = ActivatorHelper.CreateInstance<T>(args);
            ChangePage(newPage);
        }

        protected abstract void ChangePage(UserControl page);

        protected GameMenuBase()
        {
            Instance = this;
        }

#if NOESIS

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void CleanStatic()
        {
            Instance = null;
        }

#endif
    }
}