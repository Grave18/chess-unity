#if UNITY_EDITOR

using ChessBoard;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class TransferSquares : MonoBehaviour
    {
        [SerializeField] private Transform oldSquaresParent;
        [SerializeField] private Transform newSquaresParent;
        [SerializeField] private GameObject newSquarePrefab;

        [ContextMenu("Transfer")]
        public void Transfer()
        {
            foreach (Transform oldSquareTransform in oldSquaresParent)
            {
                Object newSquareObj = PrefabUtility.InstantiatePrefab(newSquarePrefab, newSquaresParent);
                GameObject newSquareInstance = newSquareObj as GameObject;
                var newSquareTransform = newSquareInstance.transform;
                newSquareTransform.position = oldSquareTransform.position;
                newSquareTransform.rotation = oldSquareTransform.rotation;
                var newSquare = newSquareInstance.GetComponent<Square>();
                var oldSquare = oldSquareTransform.GetComponent<Square>();

                newSquareInstance.name = oldSquareTransform.name.Replace("Board", "Square");
                newSquare.X = oldSquare.X;
                newSquare.Y = oldSquare.Y;
                newSquare.File = oldSquare.File;
                newSquare.Rank = oldSquare.Rank;
            }
        }
    }
}

#endif