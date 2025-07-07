namespace Ui.Auxiliary
{
    public static class LogUi
    {
        public static void Debug(object message)
        {
            LogBase(message);
        }

        private static void LogBase(object message)
        {
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.Log(message);
#else
            System.Console.WriteLine(message.ToString());
#endif
        }
    }
}
