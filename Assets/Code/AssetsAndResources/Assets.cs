using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetsAndResources
{
    public class Assets : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private BoardPreset boardPreset;
        [SerializeField] private AssetLabelReference assetLabel;

        private GameObject[] _prefabs;
        private AsyncOperationHandle<IList<GameObject>> _asyncOperationHandle;

        public BoardPreset BoardPreset => boardPreset;

        public async Task<GameObject[]> LoadPrefabs()
        {
            _asyncOperationHandle =  Addressables.LoadAssetsAsync<GameObject>(assetLabel, _ => { });
            var prefabs = await _asyncOperationHandle.Task;
            _prefabs = prefabs.ToArray();

            return _prefabs;
        }

        public ParsedPreset GetParsedPreset()
        {
            string[] splitPreset = boardPreset.Fen.Split(' ');

            var parsedPreset = new ParsedPreset
            {
                PiecesPreset = splitPreset[0],
                TurnColor = splitPreset[1],
                Castling = splitPreset[2],
                EnPassant = splitPreset[3],
                HalfMove = splitPreset[4],
                FullMove = splitPreset[5]
            };

            return parsedPreset;
        }

        public static PieceColor GetTurnColorFromPreset(ParsedPreset parsedPreset)
        {
            PieceColor turnColor = parsedPreset.TurnColor switch
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