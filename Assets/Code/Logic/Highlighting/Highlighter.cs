using System.Collections.Generic;
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
        [FormerlySerializedAs("raycastSelector")] [SerializeField] private Selector selector;

        [Header("Settings")]
        [SerializeField] private CommonPieceSettings commonSettings;

        [Header("Debug")]
        [SerializeField] private bool isDebugUnderAttackSquares;
        [SerializeField] private bool isDebugAttackLine;

        private SquareHighlighter _oldPieceHighlighter;

        private void OnEnable()
        {
            gameManager.OnTurnChanged += UpdateHighlighting;
            selector.OnClick += UpdateHighlighting;
        }

        private void OnDisable()
        {
            gameManager.OnTurnChanged -= UpdateHighlighting;
            selector.OnClick -= UpdateHighlighting;
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
            if (gameManager.CheckType is CheckType.Check or CheckType.DoubleCheck)
            {
                HighlightCheck(piece);
            }

            // Highlight selected
            if (pieceHighlighter == null)
            {
                _oldPieceHighlighter?.Show(SquareShape.None, commonSettings.DefaultColor);
                return;
            }

            HighlightSelected(pieceHighlighter);
            HighlightSquares(piece);
        }

        private void HighlightCheck(Piece piece)
        {
            // Attackers
            foreach (AttackLine attackLine in gameManager.AttackLines)
            {
                if (attackLine.IsCheck && attackLine.Attacker.TryGetComponent(out Dissolve attackerFx))
                {
                    attackerFx.HighlightPiece(enableHighlight: true);
                }
            }

            // King
            HashSet<Piece> pieces = gameManager.CurrentTurnColor == PieceColor.White ? gameManager.WhitePieces : gameManager.BlackPieces;

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
                foreach (CastlingInfo castlingInfo in king.CastlingSquares)
                {
                    if (castlingInfo.CastlingSquare.TryGetComponent(out SquareHighlighter squareHighlighter))
                    {
                        squareHighlighter.Show(SquareShape.Dot, commonSettings.CastlingColor);
                    }
                }
            }

            foreach (Square square in piece.CannotMoveSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, commonSettings.CanNotMoveColor);
                }
            }
        }

        private void HighlightSelected(SquareHighlighter pieceHighlighter)
        {
            if (_oldPieceHighlighter != pieceHighlighter)
            {
                _oldPieceHighlighter?.Show(SquareShape.None, commonSettings.DefaultColor);
                _oldPieceHighlighter = pieceHighlighter;
            }

            _oldPieceHighlighter.Show(SquareShape.Circle, commonSettings.SelectColor);
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
            Color color = gameManager.CurrentTurnColor == PieceColor.White ? Color.black : Color.white;
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
            Color color = gameManager.CurrentTurnColor == PieceColor.White ? Color.black : Color.white;
            foreach (AttackLine attackLine in gameManager.AttackLines)
            {
                if (attackLine.Attacker.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, color);
                }
            }
        }
    }
}
