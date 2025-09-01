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
        public UserControl _currentPage;

        protected GameMenuBase()
        {
            Instance = this;
        }

        public void ChangePage<T>(params object[] args) where T : UserControl
        {
            var newPage = ActivatorHelper.CreateInstance<T>(args);
            _currentPage = newPage;

            ChangePage(newPage);
        }

        protected abstract void ChangePage(UserControl page);

        public bool IsCurrentPage<T>()
        {
            return _currentPage is T;
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