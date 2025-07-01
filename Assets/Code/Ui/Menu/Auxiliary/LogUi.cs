#if UNITY_5_3_OR_NEWER
#define NOESIS
#else
using System;
#endif

namespace Ui.Menu.Auxiliary
{
    public static class LogUi
    {
        public static void Debug(object message)
        {
            LogBase(message);
        }

        private static void LogBase(object message)
        {
#if NOESIS
            UnityEngine.Debug.Log(message);
#else
            Console.WriteLine(message.ToString());
#endif
        }
    }
}
