using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Notation;
using Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetManagement
{
    public class Assets : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSettingsContainer gameSettingsContainer;

        private AsyncOperationHandle<GameObject> _boardHandle;
        private AsyncOperationHandle<IList<GameObject>> _piecesHandle;

        public async Task<(GameObject, IList<GameObject>)> LoadPrefabs()
        {
            string boardModelAddress = gameSettingsContainer.GetBoardModelAddress();
            _boardHandle = Addressables.LoadAssetAsync<GameObject>(boardModelAddress);
            GameObject boardPrefab = await _boardHandle.Task;

            string piecesModelAddress = gameSettingsContainer.GetPieceModelAddress();
            _piecesHandle =  Addressables.LoadAssetsAsync<GameObject>(piecesModelAddress, _ => { });
            IList<GameObject> piecePrefabs = await _piecesHandle.Task;

            return (boardPrefab, piecePrefabs);
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
}