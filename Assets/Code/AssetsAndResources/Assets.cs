using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessGame.Logic;
using Notation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetsAndResources
{
    public class Assets : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private AssetLabelReference assetLabel;

        private GameObject[] _prefabs;
        private AsyncOperationHandle<IList<GameObject>> _asyncOperationHandle;

        public async Task<GameObject[]> LoadPrefabs()
        {
            _asyncOperationHandle =  Addressables.LoadAssetsAsync<GameObject>(assetLabel, _ => { });
            var prefabs = await _asyncOperationHandle.Task;
            _prefabs = prefabs.ToArray();

            return _prefabs;
        }

        public static PieceColor GetTurnColorFromPreset(FenSplit fenSplit)
        {
            PieceColor turnColor = fenSplit.TurnColor switch
            {
                "w" => PieceColor.White,
                "b" => PieceColor.Black,
                _ => PieceColor.None
            };

            return turnColor;
        }

        private void OnDestroy()
        {
            ReleaseAssets();
        }

        public void ReleaseAssets()
        {
            if (_asyncOperationHandle.IsValid())
            {
                Addressables.Release(_asyncOperationHandle);
            }
        }
    }
}