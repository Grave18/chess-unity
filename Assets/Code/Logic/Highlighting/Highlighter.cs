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

        [Header("Debug")]
        [SerializeField] private bool isDebugHighlightUnderAttack;

        private void Update()
        {
            Piece piece = gameManager.Selected?.GetPiece();
            Square pieceSquare = gameManager.Selected?.GetSquare();
            var pieceHighlighter = pieceSquare?.GetComponent<SquareHighlighter>();

            ClearAllSquaresColors();

            if (isDebugHighlightUnderAttack)
            {
                DebugHighlightUnderAttack();
            }

            // Highlight selected
            if (pieceHighlighter == null)
            {
                oldPieceHighlighter?.Show(SquareShape.None, commonSettings.DefaultColor);
                return;
            }

            HighlightSelected(pieceHighlighter, piece);
            HighlightSquares(piece);

        }

        private void DebugHighlightUnderAttack()
        {
            Color color = gameManager.CurrentTurn == PieceColor.White ? Color.black : Color.white;
            foreach (Square square in gameManager.UnderAttackSquares)
            {
                var squareHighlighter = square.GetComponent<SquareHighlighter>();
                squareHighlighter.Show(SquareShape.Dot, color);
            }
        }

        private void HighlightSquares(Piece piece)
        {
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

        private void HighlightSelected(SquareHighlighter pieceHighlighter, Piece piece)
        {
            if (oldPieceHighlighter != pieceHighlighter)
            {
                oldPieceHighlighter?.Show(SquareShape.None, commonSettings.DefaultColor);
                oldPieceHighlighter = pieceHighlighter;
            }

            oldPieceHighlighter.Show(SquareShape.Circle, commonSettings.SelectColor);

            // Highlight moves and captures
            piece?.CalculateMovesAndCaptures();
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
