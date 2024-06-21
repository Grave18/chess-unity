using Board.Pieces;
using UnityEngine;

namespace Board.Settings
{
    public class CommonSettingsSetter : MonoBehaviour
    {
        [SerializeField] private bool isSetOnChange;
        [SerializeField] private CommonPieceSettings commonSettings;

        private void OnValidate()
        {
            if (!isSetOnChange)
            {
                return;
            }
            SetCommonPieceSettings();
        }

        [ContextMenu("Set Common Piece Settings")]
        private void SetCommonPieceSettings()
        {
            var pieces = FindObjectsOfType<Piece>();

            foreach (var piece in pieces)
            {
                // piece.SetCommonPieceSettings(commonSettings);
            }
        }
    }
}
