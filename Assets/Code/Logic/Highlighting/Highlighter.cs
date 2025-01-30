using System.Linq;
using Board;
using Board.Pieces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic.Highlighting
{
    public class Highlighter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private RaycastSelector raycastSelector;

        [Header("Settings")]
        [SerializeField] private CommonPieceSettings commonSettings;

        [Header("Debug")]
        [SerializeField] private bool isDebugUnderAttackSquares;
        [SerializeField] private bool isDebugAttackLine;

        private SquareHighlighter _oldPieceHighlighter;

        private void OnEnable()
        {
            gameManager.OnTurnChanged += UpdateHighlighting;
            raycastSelector.OnClick += UpdateHighlighting;
        }

        private void OnDisable()
        {
            gameManager.OnTurnChanged -= UpdateHighlighting;
            raycastSelector.OnClick -= UpdateHighlighting;
        }

        // Stab method for action
        private void UpdateHighlighting(PieceColor arg1, CheckType arg2)
        {
            UpdateHighlighting();
        }

        private void UpdateHighlighting()
        {
            Piece piece = gameManager.Selected?.GetPiece();
            Square pieceSquare = gameManager.Selected?.GetSquare();
            var pieceHighlighter = pieceSquare?.GetComponent<SquareHighlighter>();

            ClearAllHighlights();

            if (isDebugUnderAttackSquares)
            {
                DebugHighlightUnderAttackSquares();
            }

            if (isDebugAttackLine)
            {
                DebugAttackLine();
            }

            // Highlight check king and attacker pieces
            if (gameManager.CheckType == CheckType.Check)
            {
                HighlightCheck(piece);
            }

            // Highlight selected
            if (pieceHighlighter == null)
            {
                _oldPieceHighlighter?.Show(SquareShape.None, commonSettings.DefaultColor);
                return;
            }

            HighlightSelected(pieceHighlighter, piece);
            HighlightSquares(piece);
        }

        private void HighlightCheck(Piece piece)
        {
            // Attacker
            Piece attacker = gameManager.Attackers.First();
            var attackerFx = attacker.GetComponent<Dissolve>();
            attackerFx.HighlightPiece(enableHighlight: true);

            // King
            Piece[] pieces = gameManager.CurrentTurn == PieceColor.White ? gameManager.WhitePieces : gameManager.BlackPieces;

            foreach (Piece assumeKing in pieces)
            {
                if (assumeKing is not King)
                {
                    continue;
                }

                var pFx = assumeKing.GetComponent<Dissolve>();
                pFx.HighlightPiece(enableHighlight: true);

                // Highlight king square with red circle
                Square kingSquare = assumeKing.GetSquare();
                if (kingSquare.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Circle, commonSettings.CaptureColor);
                }
            }
        }

        private void HighlightSquares(Piece piece)
        {
            foreach (Square square in piece.MoveSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, commonSettings.MoveColor);
                }
            }

            foreach (Square square in piece.CaptureSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Circle, commonSettings.CaptureColor);
                }
            }

            if (piece is King king)
            {
                foreach (var square in king.CastlingSquares)
                {
                    if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                    {
                        squareHighlighter.Show(SquareShape.Dot, commonSettings.CastlingColor);
                    }
                }
            }

            foreach (Square square in piece.CannotMoveSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Cross, commonSettings.CanNotMoveColor);
                }
            }
        }

        private void HighlightSelected(SquareHighlighter pieceHighlighter, Piece piece)
        {
            if (_oldPieceHighlighter != pieceHighlighter)
            {
                _oldPieceHighlighter?.Show(SquareShape.None, commonSettings.DefaultColor);
                _oldPieceHighlighter = pieceHighlighter;
            }

            _oldPieceHighlighter.Show(SquareShape.Circle, commonSettings.SelectColor);

            // Highlight moves and captures
            piece?.CalculateMovesAndCaptures();
        }

        private void ClearAllHighlights()
        {
            foreach (Square square in gameManager.Squares)
            {
                var squareHighlighter = square.GetComponent<SquareHighlighter>();
                squareHighlighter.Show(SquareShape.None, commonSettings.DefaultColor);
            }

            foreach (Piece piece in gameManager.WhitePieces)
            {
                var pieceFx = piece.GetComponent<Dissolve>();
                pieceFx.HighlightPiece(enableHighlight: false);
            }

            foreach (Piece piece in gameManager.BlackPieces)
            {
                var pieceFx = piece.GetComponent<Dissolve>();
                pieceFx.HighlightPiece(enableHighlight: false);
            }
        }

        private void DebugHighlightUnderAttackSquares()
        {
            Color color = gameManager.CurrentTurn == PieceColor.White ? Color.black : Color.white;
            foreach (Square square in gameManager.UnderAttackSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, color);
                }
            }
        }

        private void DebugAttackLine()
        {
            Color color = gameManager.CurrentTurn == PieceColor.White ? Color.black : Color.white;
            foreach (Square square in gameManager.AttackLine)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, color);
                }
            }
        }
    }
}
