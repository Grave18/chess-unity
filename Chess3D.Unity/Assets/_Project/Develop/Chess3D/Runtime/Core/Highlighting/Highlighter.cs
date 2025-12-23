using System;
using Chess3D.Runtime.Core.ChessBoard;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.Highlighting
{
    public sealed class Highlighter: MonoBehaviour
    {
        [SerializeField] private SquareColors squareColors;

        private Game _game;
        private Board _board;

        [Inject]
        public void Construct(Game game, Board board, CoreEvents coreEvents)
        {
            _game = game;
            _board = board;

            coreEvents.OnWarmup.Subscribe(_ => ClearAllHighlights()).AddTo(this);
            coreEvents.OnStart.Subscribe(UpdateHighlighting).AddTo(this);
            coreEvents.OnEndMove.Subscribe(UpdateHighlighting).AddTo(this);
            coreEvents.OnSelect.Subscribe(UpdateHighlighting).AddTo(this);
        }

        private void UpdateHighlighting(Unit _ = default)
        {
            ClearAllHighlights();
            HighlightCheck();
            HighlightSquaresUnderSelected();
            HighlightSquaresConnectedWithSelectedPiece();
        }

        private void HighlightSquaresUnderSelected()
        {
            Square pieceSquare = _game.Selected?.GetSquare();
            var squareHighlighter = pieceSquare?.GetComponent<SquareHighlighter>();

            squareHighlighter?.Show(SquareShape.Circle, squareColors.SelectColor);
        }

        private void HighlightCheck()
        {
            foreach (AttackLine attackLine in _game.AttackLines)
            {
                if (attackLine.IsCheck)
                {
                    var attackerSquareHighlighter = attackLine.Attacker.GetSquare().GetComponent<SquareHighlighter>();
                    attackerSquareHighlighter?.Show(SquareShape.Circle, squareColors.CheckColor);

                    // HighLight king square
                    foreach (Piece assumeKing in _game.CurrentTurnPieces)
                    {
                        if (assumeKing is not King)
                        {
                            continue;
                        }

                        // Highlight king square with red circle
                        Square kingSquare = assumeKing.GetSquare();
                        if (kingSquare.TryGetComponent(out SquareHighlighter kingSquareHighlighter))
                        {
                            kingSquareHighlighter.Show(SquareShape.Circle, squareColors.CheckColor);
                        }
                    }
                }
            }
        }

        private void HighlightSquaresConnectedWithSelectedPiece()
        {
            Piece piece = _game.Selected?.GetPiece();

            if (piece == null)
            {
                return;
            }

            foreach ((Square square, _) in piece.MoveSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, squareColors.MoveColor);
                }
            }

            foreach ((Square square, _) in piece.CaptureSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Circle, squareColors.CaptureColor);
                }
            }

            if (piece is King king)
            {
                foreach (CastlingInfo castlingInfo in king.CastlingSquares)
                {
                    if (castlingInfo.KingToSquare.TryGetComponent(out SquareHighlighter squareHighlighter))
                    {
                        squareHighlighter.Show(SquareShape.Dot, squareColors.CastlingColor);
                    }
                }
            }

            foreach (Square square in piece.CannotMoveSquares)
            {
                if (square.TryGetComponent(out SquareHighlighter squareHighlighter))
                {
                    squareHighlighter.Show(SquareShape.Dot, squareColors.CanNotMoveColor);
                }
            }
        }

        private void ClearAllHighlights()
        {
            foreach (Square square in _board.Squares)
            {
                var squareHighlighter = square.GetComponent<SquareHighlighter>();
                squareHighlighter.Show(SquareShape.None, squareColors.DefaultColor);
            }
        }

        [Serializable]
        public sealed class SquareColors
        {
            [Header("Highlight Colors")]
            public Color DefaultColor = Color.black;
            public Color SelectColor = Color.green;
            public Color CheckColor = Color.magenta;
            public Color MoveColor = Color.green;
            public Color CaptureColor = Color.red;
            public Color CanNotMoveColor = Color.gray;
            public Color CastlingColor = Color.yellow;
        }
    }
}
