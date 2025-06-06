using System.Collections.Generic;
using ChessGame.ChessBoard.Info;
using ChessGame.Logic;
using EditorCools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BoardEditor
{
    public class VisualBoardSpawner : MonoBehaviour
    {
        [Header("Board")] [SerializeField] private AssetLabelReference boardLabel;
        [SerializeField] private Transform boardParent;

        [Header("Pieces")] [SerializeField] private AssetLabelReference piecesLabel;
        [SerializeField] private PieceSpawnPoint[] pieceSpawnPoints;

        [Header("White Pieces")] [SerializeField]
        private Transform[] whiteKings;

        private AsyncOperationHandle<GameObject> _boardAssetHandle;
        private AsyncOperationHandle<IList<GameObject>> _piecesAssetHandle;

        [Button(space: 10)]
        public void LoadBoard()
        {
            if (_boardAssetHandle.IsValid())
            {
                Addressables.Release(_boardAssetHandle);
            }

            _boardAssetHandle = Addressables.InstantiateAsync(boardLabel, boardParent);
        }

        [Button(space: 5)]
        public async void LoadPieces()
        {
            try
            {
                if (_piecesAssetHandle.IsValid())
                {
                    Addressables.Release(_piecesAssetHandle);
                }

                _piecesAssetHandle = Addressables.LoadAssetsAsync<GameObject>(piecesLabel, _ => { });

                IList<GameObject> prefabs = await _piecesAssetHandle.Task;

                foreach (PieceSpawnPoint point in pieceSpawnPoints)
                {
                    InstantiatePiece(point, prefabs);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void InstantiatePiece(PieceSpawnPoint point, IList<GameObject> prefabs)
        {
            switch (point.PieceColor)
            {
                case PieceColor.Black:
                    switch (point.PieceType)
                    {
                        case PieceType.Bishop: Instantiate(prefabs[0], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.King: Instantiate(prefabs[1], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.Knight: Instantiate(prefabs[2], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.Pawn: Instantiate(prefabs[3], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.Queen: Instantiate(prefabs[4], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.Rook: Instantiate(prefabs[5], point.transform.position, point.transform.rotation, point.transform); break;
                    }
                    break;

                case PieceColor.White:
                    switch (point.PieceType)
                    {
                        case PieceType.Bishop: Instantiate(prefabs[6], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.King: Instantiate(prefabs[7], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.Knight: Instantiate(prefabs[8], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.Pawn: Instantiate(prefabs[9], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.Queen: Instantiate(prefabs[10], point.transform.position, point.transform.rotation, point.transform); break;
                        case PieceType.Rook: Instantiate(prefabs[11], point.transform.position, point.transform.rotation, point.transform); break;
                    }
                    break;
            }
        }
    }
}