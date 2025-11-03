using System.Collections.Generic;
using Chess3D.Runtime.Core.ChessBoard;
using EditorCools;
using UnityEngine;

#if UNITY_EDITOR

namespace Ui.BoardInMainMenu
{
    public class PiecePointSpawnerInEditor : MonoBehaviour
    {
        [SerializeField] private Transform piecesRoot;
        [SerializeField] private GameObject pieceSpawnPointPrefab;

        [SerializeField] private Square[] squares;

        private readonly List<GameObject> _points = new();
        [Button(space: 10)]
        public void SpawnPiecePoints()
        {
            foreach (Square square in squares)
            {
                if (square.Y <= 1)
                {
                    GameObject point = Instantiate(pieceSpawnPointPrefab, square.transform.position, Quaternion.identity, piecesRoot);
                    point.name = "WhitePoint";
                    _points.Add(point);
                }
                else if (square.Y >= 6)
                {
                    Quaternion rotation = Quaternion.Euler(0, 180, 0);
                    GameObject point = Instantiate(pieceSpawnPointPrefab, square.transform.position, rotation, piecesRoot);
                    point.name = "BlackPoint";
                    _points.Add(point);
                }
            }
        }

        [Button(space: 5)]
        public void RemovePiecePoints()
        {
            foreach (GameObject point in _points)
            {
                DestroyImmediate(point);
            }

            _points.Clear();
        }
    }
}

#endif