using System;
using System.Runtime.CompilerServices;
using Noesis;
using UnityEngine;
using GUI = Noesis.GUI;

namespace Chess3D.Runtime.Utilities
{
    public static class XamlUtils
    {
        public static string GetXamlPathFromFilePath([CallerFilePath]string filePath = "")
        {
            // Remove .cs
            string xamlPath = filePath.Replace(".cs", "");

            // Replace \ with /
            xamlPath = xamlPath.Replace(@"\", "/");

            // Find "Assets/" relative part
            int assetsIndex = xamlPath.IndexOf("Assets/", StringComparison.OrdinalIgnoreCase);
            if (assetsIndex == -1)
            {
                Debug.LogError($"Cannot locate 'Assets/' in path: {xamlPath}");
                return "";
            }

            // Strip to ../Assets/
            xamlPath = xamlPath.Substring(assetsIndex);

            return xamlPath;
        }

        public static void Load(UserControl @this)
        {
            GUI.LoadComponent(@this, GetXamlPathFromFilePath());
        }
    }
}