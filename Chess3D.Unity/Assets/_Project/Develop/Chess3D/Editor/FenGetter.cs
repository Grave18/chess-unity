using Chess3D.Runtime.Core;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Utilities.Logging;
using UnityEditor;

namespace Chess3D.Editor
{
    public static class FenGetter
    {
        [MenuItem("Tools/Grave/Get Fen")]
        public static void GetFen()
        {
            if (!ServiceLocator.IsValid)
            {
                Log.Editor.W("Must use in Play mode when board is loaded!");
                return;
            }

            var fenFromBoard = ServiceLocator.Resolve<FenFromBoard>();
            string fen = fenFromBoard.Get();
            EditorGUIUtility.systemCopyBuffer = fen;
            Log.Editor.D(fen + " Copied to to clipboard");
        }
    }
}