using ChessBoard.Builder;
using UnityEngine;
using UnityEditor;

namespace Tools
{
    public class BoardEditor : EditorWindow
    {
        private static Board _board;
        private BoardPreset _boardPreset;

        [MenuItem("Tools/Board Editor")]
        public static void Open()
        {
            GetWindow<BoardEditor>();
        }

        private void OnGUI()
        {
            Setup();
            GUIStyle buttonStyle = GetButtonStyle();

            GUILayout.Space(20f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _boardPreset = GetBoardPreset();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Build Board", buttonStyle))
            {
                BuildBoard();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Destroy all pieces", buttonStyle))
            {
                _board.DestroyAllPieces();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private static void Setup()
        {
            _board = FindFirstObjectByType<Board>();

            if(_board == null)
            {
                Debug.LogError("Board not found. Make sure there is a Board object in the scene.");
            }
        }

        private static GUIStyle GetButtonStyle()
        {
            return new GUIStyle(GUI.skin.button)
            {
                fontSize = 18,
                fixedWidth = 250,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                stretchHeight = true,
            };
        }

        private BoardPreset GetBoardPreset()
        {
            return (BoardPreset)EditorGUILayout.ObjectField(
                "Board preset", _boardPreset, typeof(BoardPreset), false, GUILayout.MaxWidth(350f));
        }

        private void BuildBoard()
        {
            if (_boardPreset == null)
            {
                Debug.LogError("Board preset not set.");
            }

            _ = _board.BuildBoardAsync(_boardPreset);
        }
    }
}