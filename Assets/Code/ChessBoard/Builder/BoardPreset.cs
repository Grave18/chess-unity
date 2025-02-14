using Logic;
using UnityEngine;

namespace ChessBoard.Builder
{
    [CreateAssetMenu(fileName = "BoardPreset", menuName = "Board/BoardPreset")]
    public class BoardPreset : ScriptableObject
    {
        [Header("Preset")]
        [SerializeField] private PieceColor turnColor;

        [Space]
        [TextArea(8, 8)]
        [SerializeField] private string preset;

        public string Preset => preset;

        public PieceColor TurnColor => turnColor;
    }
}
