using UnityEngine;
using UnityEngine.Serialization;

namespace AssetManagement
{
    [CreateAssetMenu(fileName = "BoardPreset", menuName = "Board/BoardPreset")]
    public class BoardPreset : ScriptableObject
    {
        [TextArea(8, 8)]
        [FormerlySerializedAs("preset")]
        [SerializeField] private string fen;

        public string Fen => fen;
    }
}
