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
            Piece piece = gameManager.Selected?.GetPiece();
            var pieceRenderer = piece?.GetComponent<MeshRenderer>();
            ClearAllSquaresColors();

            // Highlight selected
            if (pieceRenderer == null)
            {
                _renderer?.material.SetColor(EmissionColor, commonSettings.DefaultColor);

                return;
            }

            if (_renderer != pieceRenderer)
            {
                _renderer?.material.SetColor(EmissionColor, commonSettings.DefaultColor);
                _renderer = pieceRenderer;
            }

            _renderer.material.SetColor(EmissionColor, commonSettings.SelectColor);

            // Highlight moves and captures
            piece.CalculateMovesAndCaptures();


            foreach (Square square in piece.MoveSquares)
            {
                var squareRenderer = square.GetComponent<MeshRenderer>();
                squareRenderer.material.SetColor(EmissionColor, commonSettings.MoveColor);
            }

            foreach (Square square in piece.CaptureSquares)
            {
                var squareRenderer = square.GetComponent<MeshRenderer>();
                squareRenderer.material.SetColor(EmissionColor, commonSettings.CaptureColor);
            }

            if (piece is King king)
            {
                foreach (Square square in king.CannotMoveSquares)
                {
                    var squareRenderer = square.GetComponent<MeshRenderer>();
                    squareRenderer.material.SetColor(EmissionColor, commonSettings.CanNotMoveColor);
                }

                foreach (var square in king.CastlingSquares)
                {
                    var squareRenderer = square.GetComponent<MeshRenderer>();
                    squareRenderer.material.SetColor(EmissionColor, commonSettings.MoveColor);
                }
            }
        }

        private void ClearAllSquaresColors()
        {
            foreach (var square in gameManager.Squares)
            {
                var squareRenderer = square.GetComponent<MeshRenderer>();
                squareRenderer.material.SetColor(EmissionColor, commonSettings.DefaultColor);
            }
        }
    }
}
