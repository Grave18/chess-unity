using System;
using System.Collections.Generic;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Scripting;

namespace Chess3D.Runtime.Core.AssetManagement
{
    [Preserve]
    public sealed class Assets : ILoadUnit, IDisposable
    {
        public LoadedPrefabs Prefabs;

        private readonly SettingsService _settingsService;

        private AsyncOperationHandle<GameObject> _boardHandle;
        private AsyncOperationHandle<IList<GameObject>> _piecesHandle;

        public Assets(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async UniTask Load()
        {
            string boardModelAddress = _settingsService.S.GameSettings.BoardModelAddress;
            _boardHandle = Addressables.LoadAssetAsync<GameObject>(boardModelAddress);
            GameObject boardPrefab = await _boardHandle;

            string piecesModelAddress = _settingsService.S.GameSettings.PiecesModelAddress;
            _piecesHandle =  Addressables.LoadAssetsAsync<GameObject>(piecesModelAddress, _ => { });
            IList<GameObject> piecePrefabs = await _piecesHandle;

            Prefabs = new LoadedPrefabs
            {
                Board = boardPrefab,
                Pieceses = piecePrefabs
            };
        }

        public void Dispose()
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
        public GameObject Board;
        public IList<GameObject> Pieceses;
    }
}