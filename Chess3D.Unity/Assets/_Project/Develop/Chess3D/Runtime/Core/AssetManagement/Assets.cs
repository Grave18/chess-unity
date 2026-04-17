using System;
using System.Collections.Generic;
using System.Threading;
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

        private readonly ISettingsContainer settingsContainer;

        private AsyncOperationHandle<GameObject> _boardHandle;
        private AsyncOperationHandle<IList<GameObject>> _piecesHandle;

        public Assets(ISettingsContainer settingsContainer)
        {
            this.settingsContainer = settingsContainer;
        }

        public async UniTask Load(CancellationToken cancellationToken = default)
        {
            try
            {
                string boardModelAddress = settingsContainer.GameSettings.BoardModelAddress;
                _boardHandle = Addressables.LoadAssetAsync<GameObject>(boardModelAddress);
                GameObject boardPrefab = await _boardHandle.ToUniTask(cancellationToken: cancellationToken);

                string piecesModelAddress = settingsContainer.GameSettings.PiecesModelAddress;
                _piecesHandle =  Addressables.LoadAssetsAsync<GameObject>(piecesModelAddress, _ => { });
                IList<GameObject> piecePrefabs = await _piecesHandle.ToUniTask(cancellationToken: cancellationToken);

                Prefabs = new LoadedPrefabs
                {
                    Board = boardPrefab,
                    Pieceses = piecePrefabs
                };
            }
            catch (OperationCanceledException)
            {
                ReleaseAssets();
                throw;
            }
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