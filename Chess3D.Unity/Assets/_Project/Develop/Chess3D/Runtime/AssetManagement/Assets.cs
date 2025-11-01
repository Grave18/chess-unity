using System.Collections.Generic;
using Chess3D.Runtime.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Chess3D.Runtime.AssetManagement
{
    public class Assets : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private AsyncOperationHandle<GameObject> _boardHandle;
        private AsyncOperationHandle<IList<GameObject>> _piecesHandle;

        public async UniTask<LoadedPrefabs> LoadPrefabs()
        {
            string boardModelAddress = gameSettingsContainer.GetBoardModelAddress();
            _boardHandle = Addressables.LoadAssetAsync<GameObject>(boardModelAddress);
            GameObject boardPrefab = await _boardHandle;

            string piecesModelAddress = gameSettingsContainer.GetPieceModelAddress();
            _piecesHandle =  Addressables.LoadAssetsAsync<GameObject>(piecesModelAddress, _ => { });
            IList<GameObject> piecePrefabs = await _piecesHandle;

            return new LoadedPrefabs { BoardPrefab = boardPrefab, PiecesPrefabs = piecePrefabs };
        }

        private void OnDestroy()
        {
            ReleaseAssets();
        }

        public void ReleaseAssets()
        {
            if (_piecesHandle.IsValid())
            {
                Addressables.Release(_piecesHandle);
            }

            if (_boardHandle.IsValid())
            {
                Addressables.Release(_boardHandle);
            }
        }
    }

    public struct LoadedPrefabs
    {
        public GameObject BoardPrefab;
        public IList<GameObject> PiecesPrefabs;
    }
}