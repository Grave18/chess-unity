#if UNITY_5_3_OR_NEWER
#define NOESIS
using UnityEngine;
using Noesis;

#else
using System.Windows.Controls;
#endif

using System;

namespace Ui.Auxiliary
{
    public abstract partial class GameMenuBase : UserControl
    {
        public static GameMenuBase Instance { get; private set; }

        public void ChangePage<T>() where T : UserControl
        {
            var newPage = Activator.CreateInstance<T>();
            ChangePage(newPage);
        }

        protected abstract void ChangePage(UserControl page);

        protected GameMenuBase()
        {
            if (Instance != null)
            {
                LogUi.Debug($"{typeof(GameMenuBase)} already bound. Replacing Instance:");
            }

            Instance = this;
            LogUi.Debug($"Bound {typeof(GameMenuBase)} to {Instance}");
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