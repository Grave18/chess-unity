using Board;
using Board.Pieces;
using UnityEngine;

namespace Logic.Highlighting
{
    public class Highlighter : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private SquareHighlighter oldPieceHighlighter;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private CommonPieceSettings commonSettings;

        private void Update()
        {
            Piece piece = gameManager.Selected?.GetPiece();
            Square pieceSquare = gameManager.Selected?.GetSquare();
            var pieceHighlighter = pieceSquare?.GetComponent<SquareHighlighter>();
            ClearAllSquaresColors();

            // Highlight selected
            if (pieceHighlighter == null)
            {
                oldPieceHighlighter?.Show(SquareShape.None, commonSettings.DefaultColor);
                return;
            }

            if (oldPieceHighlighter != pieceHighlighter)
            {
                oldPieceHighlighter?.Show(SquareShape.None, commonSettings.DefaultColor);
                oldPieceHighlighter = pieceHighlighter;
            }

            oldPieceHighlighter.Show(SquareShape.Circle, commonSettings.SelectColor);

            // Highlight moves and captures
            piece?.CalculateMovesAndCaptures();


            foreach (Square square in piece.MoveSquares)
            {
                var squareHighlighter = square.GetComponent<SquareHighlighter>();
                squareHighlighter.Show(SquareShape.Dot, commonSettings.MoveColor);
            }

            foreach (Square square in piece.CaptureSquares)
            {
                var squareHighlighter = square.GetComponent<SquareHighlighter>();
                squareHighlighter.Show(SquareShape.Circle, commonSettings.CaptureColor);
            }

            if (piece is King king)
            {
                foreach (Square square in king.CannotMoveSquares)
                {
                    var squareHighlighter = square.GetComponent<SquareHighlighter>();
                    squareHighlighter.Show(SquareShape.Cross, commonSettings.CanNotMoveColor);
                }

                foreach (var square in king.CastlingSquares)
                {
                    var squareHighlighter = square.GetComponent<SquareHighlighter>();
                    squareHighlighter.Show(SquareShape.Dot, commonSettings.MoveColor);
                }
            }
        }

        private void ClearAllSquaresColors()
        {
            foreach (var square in gameManager.Squares)
            {
                var squareHighlighter = square.GetComponent<SquareHighlighter>();
                squareHighlighter.Show(SquareShape.None, commonSettings.DefaultColor);
            }
        }
    }
}
