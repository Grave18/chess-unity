using System.Collections.Generic;
using Chess3D.Runtime.ChessBoard.Info;
using Chess3D.Runtime.Logic;
using Chess3D.Runtime.Settings;
using EditorCools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Ui.BoardInMainMenu
{
    public class VisualBoardSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        [Header("Board")]
        [SerializeField] private AssetLabelReference boardLabel;
        [SerializeField] private Transform boardParent;

        [Header("Pieces")]
        [SerializeField] private AssetLabelReference piecesLabel;
        [SerializeField] private PieceSpawnPoint[] pieceSpawnPoints;

        private AsyncOperationHandle<GameObject> _boardHandle;
        private GameObject _boardInstance;

        private AsyncOperationHandle<IList<GameObject>> _piecesHandle;
        private readonly List<GameObject> _pieceInstances = new();

        private void Awake()
        {
            string boardModelAddress = gameSettingsContainer.GetBoardModelAddress();
            SpawnBoard(boardModelAddress);

            string pieceModelAddress = gameSettingsContainer.GetPieceModelAddress();
            SpawnPieces(pieceModelAddress);
        }

        public void SpawnBoard(string boardModelAddress)
        {
            if (_boardInstance)
            {
                Addressables.ReleaseInstance(_boardInstance);
            }

            _boardHandle = Addressables.InstantiateAsync(boardModelAddress, boardParent);
            _boardHandle.Completed += OnBoardLoaded;
        }

        private void OnBoardLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"{nameof(VisualBoardSpawner)}: Failed to load board\n" +
                               $"Status: {handle.Status}\n" +
                               $"Error: {handle.OperationException}");
                return;
            }

            _boardInstance = handle.Result;
        }

        public void SpawnPieces(string address)
        {
            foreach (GameObject piece in _pieceInstances)
            {
                Destroy(piece);
            }

            if (_piecesHandle.IsValid())
            {
                Addressables.Release(_piecesHandle);
            }

            _piecesHandle = Addressables.LoadAssetsAsync<GameObject>(address, _ => { });
            _piecesHandle.Completed += OnPiecesLoaded;
        }

        private void OnPiecesLoaded(AsyncOperationHandle<IList<GameObject>> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"{nameof(VisualBoardSpawner)}: Failed to load pieces\n" +
                               $"Status: {handle.Status}\n" +
                               $"Error: {handle.OperationException}");
                return;
            }

            IList<GameObject> prefabs = handle.Result;
            foreach (PieceSpawnPoint point in pieceSpawnPoints)
            {
                InstantiatePiece(point, prefabs);
            }
        }

        private void InstantiatePiece(PieceSpawnPoint point, IList<GameObject> prefabs)
        {
            GameObject currentPrefab = point.PieceColor switch
            {
                PieceColor.Black => point.PieceType switch
                {
                    PieceType.Bishop => prefabs[0],
                    PieceType.King => prefabs[1],
                    PieceType.Knight => prefabs[2],
                    PieceType.Pawn => prefabs[3],
                    PieceType.Queen => prefabs[4],
                    PieceType.Rook => prefabs[5],
                    _ => null,
                },
                PieceColor.White => point.PieceType switch
                {
                    PieceType.Bishop => prefabs[6],
                    PieceType.King => prefabs[7],
                    PieceType.Knight => prefabs[8],
                    PieceType.Pawn => prefabs[9],
                    PieceType.Queen => prefabs[10],
                    PieceType.Rook => prefabs[11],
                    _ => null,
                },
                _ => null
            };

            if (currentPrefab)
            {
                GameObject instance = Instantiate(currentPrefab, point.transform.position, point.transform.rotation, point.transform);
                _pieceInstances.Add(instance);
            }
        }

    #if UNITY_EDITOR

        [Button(space: 10)]
        public void LoadPieces()
        {
            SpawnPieces(piecesLabel.labelString);
        }

        [Button(space: 5)]
        public void LoadBoard()
        {
            SpawnBoard(boardLabel.labelString);
        }

#endif
    }
}