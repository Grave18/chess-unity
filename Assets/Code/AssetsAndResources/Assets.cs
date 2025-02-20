using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public BoardPreset Preset => boardPreset;

        public async Task<GameObject[]> InitAsync()
        {
            _asyncOperationHandle =  Addressables.LoadAssetsAsync<GameObject>(assetLabel, _ => { });
            var prefabs = await _asyncOperationHandle.Task;
            _prefabs = prefabs.ToArray();

            return _prefabs;
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