using System;
using System.IO;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsCommon
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class LayoutLoader
    {
        private static readonly Type WindowLayoutType;

#if UNITY_EDITOR
        static LayoutLoader()
        {
            WindowLayoutType = typeof(Editor).Assembly.GetType("UnityEditor.WindowLayout");
            if (WindowLayoutType == null)
            {
                Debug.LogError("Could not find UnityEditor.WindowLayout");
            }
        }
#endif
        public static void Load(string layoutName)
        {
            string layoutPath = GetLayoutPath(layoutName);

            SetLayout(layoutPath);
        }

        private static string GetLayoutPath(string layoutName)
        {
            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string layoutDir = Path.Combine(roamingPath, @"Unity\Editor-5.x\Preferences\Layouts\default\");
            string layoutPath = Path.Combine(layoutDir, layoutName + ".wlt");

            if (!File.Exists(layoutPath))
            {
                Debug.LogError("Layout file not found: " + layoutPath);
                return string.Empty;
            }

            Debug.Log("Layout path: " + layoutPath);

            return layoutPath;
        }

        // Unfortunately returns .dwlt not .wlt
        private static string GetCurrentLayoutPath()
        {
            MethodInfo getCurrentLayoutPath = WindowLayoutType.GetMethod("GetCurrentLayoutPath",
                BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { }, null);

            if (getCurrentLayoutPath == null)
            {
                Debug.LogError("Could not find GetCurrentLayoutPath()");
                return null;
            }

            string currentLayoutPath = getCurrentLayoutPath.Invoke(null, null) as string;
            Debug.Log("Current layout path: " + currentLayoutPath);
            return currentLayoutPath;
        }

        private static void SetLayout(string layoutPath)
        {
            MethodInfo loadWindowLayout = WindowLayoutType.GetMethod(
                "LoadWindowLayout",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new[] { typeof(string), typeof(bool) },
                null);

            if (loadWindowLayout == null)
            {
                Debug.LogError("Could not find LoadWindowLayout(string, bool)");
                return;
            }

            loadWindowLayout.Invoke(null, new object[] { layoutPath, false });
            Debug.Log("Layout loaded from: " + layoutPath);
        }
    }
}