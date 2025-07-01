#if UNITY_5_3_OR_NEWER
#define NOESIS
#else
using System;
#endif

namespace Ui.Menu.Auxiliary
{
    public static class Log
    {
        public static void Debug(string message)
        {
            LogBase(message);
        }

        private static void LogBase(string message)
        {
#if NOESIS
            UnityEngine.Debug.Log(message);
#else
            Console.WriteLine(message);
#endif
        }
    }
}
