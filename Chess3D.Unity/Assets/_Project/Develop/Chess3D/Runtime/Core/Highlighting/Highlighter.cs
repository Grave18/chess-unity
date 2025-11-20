using System;
using UnityEngine;
using UnityEngine.Scripting;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Effects;
using Chess3D.Runtime.Core.Logic;

namespace Chess3D.Runtime.Core.Highlighting
{
    [Preserve]
    public class Highlighter: IDisposable
    {
        private readonly Game _game;
        private readonly Board _board;
        private readonly CoreEvents _coreEvents;
        private readonly CommonPieceSettings _commonSettings;

        private bool isDebugUnderAttackSquares;
        private bool isDebugAttackLine;

        private SquareHighlighter _oldPieceHighlighter;

        public Highlighter(Game game, Board board, CoreEvents coreEvents, CommonPieceSettings commonSettings)
        {
            _game = game;
            _board = board;
            _coreEvents = coreEvents;
            _commonSettings = commonSettings;

            coreEvents.OnEndMove += UpdateHighlighting;
        }

        public void Dispose()
        {
            if (_coreEvents is null)
            {
                return;
            }

            _coreEvents.OnEndMove -= UpdateHighlighting;
        }

        public void UpdateHighlighting()
        {
            Piece piece = _game.Selected?.GetPiece();
            Square pieceSquare = _game.Selected?.GetSquare();
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
            if (_game.IsCheck)
            {
                HighlightCheck(piece);
            }

            // Highlight selected
            if (pieceHighlighter == null)
            {
                _oldPieceHighlighter?.Show(SquareShape.None, _commonSettings.DefaultColor);
                return;
            }

            HighlightSelected(pieceHighlighter);
            HighlightSquares(piece);
        }

        private void HighlightCheck(Piece piece)
        {
            // Attackers
            foreach (AttackLine attackLine in _game.AttackLines)
            {
                if (attackLine.IsCheck && attackLine.Attacker.TryGetComponent(out EffectsRunner attackerFx))
                {
                    attackerFx.HighlightPiece(enableHighlight: true);
                }
            }

            // King
            foreach (Piece assumeKing in _game.CurrentTurnPieces)
            {
                if (assumeKing is not King)
                {
                    continue;
                }

                var pFx = assumeKing.GetComponent<EffectsRunner>();
                pFx.HighlightPiece(enableHighlight: true);

                // Highlight king square with red circle
                Square kingSquare = assumeKing.GetSquare();
                if (kingSquare.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Circle, _commonSettings.CaptureColor);
                }
            }
        }

        private void HighlightSquares(Piece piece)
        {
            foreach ((Square square, _) in piece.MoveSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, _commonSettings.MoveColor);
                }
            }

            foreach ((Square square, _) in piece.CaptureSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Circle, _commonSettings.CaptureColor);
                }
            }

            if (piece is King king)
            {
                foreach (CastlingInfo castlingInfo in king.CastlingSquares)
                {
                    if (castlingInfo.KingToSquare.TryGetComponent(out SquareHighlighter squareHighlighter))
                    {
                        squareHighlighter.Show(SquareShape.Dot, _commonSettings.CastlingColor);
                    }
                }
            }

            foreach (Square square in piece.CannotMoveSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, _commonSettings.CanNotMoveColor);
                }
            }
        }

        private void HighlightSelected(SquareHighlighter pieceHighlighter)
        {
            if (_oldPieceHighlighter != pieceHighlighter)
            {
                _oldPieceHighlighter?.Show(SquareShape.None, _commonSettings.DefaultColor);
                _oldPieceHighlighter = pieceHighlighter;
            }

            _oldPieceHighlighter.Show(SquareShape.Circle, _commonSettings.SelectColor);
        }

        private void ClearAllHighlights()
        {
            foreach (Square square in _board.Squares)
            {
                var squareHighlighter = square.GetComponent<SquareHighlighter>();
                squareHighlighter.Show(SquareShape.None, _commonSettings.DefaultColor);
            }

            foreach (Piece piece in _board.WhitePieces)
            {
                var pieceFx = piece.GetComponent<EffectsRunner>();
                pieceFx.HighlightPiece(enableHighlight: false);
            }

            foreach (Piece piece in _board.BlackPieces)
            {
                var pieceFx = piece.GetComponent<EffectsRunner>();
                pieceFx.HighlightPiece(enableHighlight: false);
            }
        }

        private void DebugHighlightUnderAttackSquares()
        {
            Color color = _game.CurrentTurnColor == PieceColor.White ? Color.black : Color.white;
            foreach (Square square in _game.UnderAttackSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, color);
                }
            }
        }

        private void DebugAttackLine()
        {
            Color color = _game.CurrentTurnColor == PieceColor.White ? Color.black : Color.white;
            foreach (AttackLine attackLine in _game.AttackLines)
            {
                if (attackLine.Attacker.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, color);
                }
            }
        }
    }
}
