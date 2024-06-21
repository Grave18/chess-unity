using Logic;
using UnityEngine;

namespace Board.Pieces
{
    public class Highlighter : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private MeshRenderer _renderer;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private CommonPieceSettings commonSettings;

        private void Update()
        {
            var renderer = gameManager.Selected?.GetPiece().GetComponent<MeshRenderer>();

            if (renderer == null)
            {
                _renderer?.material.SetColor(EmissionColor, commonSettings.DefaultColor);
                return;
            }

            if (_renderer != renderer)
            {
                _renderer?.material.SetColor(EmissionColor, commonSettings.DefaultColor);
                _renderer = renderer;
            }

            _renderer.material.SetColor(EmissionColor, commonSettings.SelectColor);
        }
    }
}
