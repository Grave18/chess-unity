using UnityEngine;

namespace AssetsAndResources
{
    [CreateAssetMenu(fileName = "BoardPreset", menuName = "Board/BoardPreset")]
    public class BoardPreset : ScriptableObject
    {
        [TextArea(8, 8)]
        [SerializeField] private string preset;

        public string Preset => preset;
    }
}
