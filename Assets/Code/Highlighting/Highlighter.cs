using System.Collections.Generic;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic;
using Logic.Players;
using UnityEngine;

namespace Highlighting
{
    public class Highlighter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Game game;
        [SerializeField] private PlayerOffline playerOffline;

        [Header("Settings")]
        [SerializeField] private CommonPieceSettings commonSettings;

        [Header("Debug")]
        [SerializeField] private bool isDebugUnderAttackSquares;
        [SerializeField] private bool isDebugAttackLine;

        private SquareHighlighter _oldPieceHighlighter;

        private void OnEnable()
        {
            game.OnEndTurn += UpdateHighlighting;
            playerOffline.OnClick += UpdateHighlighting;
        }

        private void OnDisable()
        {
            game.OnEndTurn -= UpdateHighlighting;
            playerOffline.OnClick -= UpdateHighlighting;
        }

        // Stab method for action
        private void UpdateHighlighting(PieceColor arg1, CheckType arg2)
        {
            UpdateHighlighting();
        }

        private void UpdateHighlighting()
        {
            Piece piece = game.Selected?.GetPiece();
            Square pieceSquare = game.Selected?.GetSquare();
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
            if (game.CheckType is CheckType.Check or CheckType.DoubleCheck)
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
            foreach (AttackLine attackLine in game.AttackLines)
            {
                if (attackLine.IsCheck && attackLine.Attacker.TryGetComponent(out Dissolve attackerFx))
                {
                    attackerFx.HighlightPiece(enableHighlight: true);
                }
            }

            // King
            HashSet<Piece> pieces = game.CurrentTurnColor == PieceColor.White ? game.WhitePieces : game.BlackPieces;

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
            foreach ((Square square, _) in piece.MoveSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, commonSettings.MoveColor);
                }
            }

            foreach ((Square square, _) in piece.CaptureSquares)
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
            foreach (Square square in game.Squares)
            {
                var squareHighlighter = square.GetComponent<SquareHighlighter>();
                squareHighlighter.Show(SquareShape.None, commonSettings.DefaultColor);
            }

            foreach (Piece piece in game.WhitePieces)
            {
                var pieceFx = piece.GetComponent<Dissolve>();
                pieceFx.HighlightPiece(enableHighlight: false);
            }

            foreach (Piece piece in game.BlackPieces)
            {
                var pieceFx = piece.GetComponent<Dissolve>();
                pieceFx.HighlightPiece(enableHighlight: false);
            }
        }

        private void DebugHighlightUnderAttackSquares()
        {
            Color color = game.CurrentTurnColor == PieceColor.White ? Color.black : Color.white;
            foreach (Square square in game.UnderAttackSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, color);
                }
            }
        }

        private void DebugAttackLine()
        {
            Color color = game.CurrentTurnColor == PieceColor.White ? Color.black : Color.white;
            foreach (AttackLine attackLine in game.AttackLines)
            {
                if (attackLine.Attacker.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, color);
                }
            }
        }
    }
}
