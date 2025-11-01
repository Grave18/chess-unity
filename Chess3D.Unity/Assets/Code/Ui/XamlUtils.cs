using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ui
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
    }
}