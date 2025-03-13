using System.Collections.Generic;
using ChessBoard.Info;
using UnityEngine;

namespace ChessBoard.Pieces
{
    public class LongRange : Piece
    {
        [Header("Long Range")]
        [SerializeField] private Vector2Int[] moveVectors;

        public HashSet<Square> AttackLineSquares { get; private set; } = new();
        public Piece PinnedPiece { get; private set; }

        private int _pinnedPieceCount;

        public bool HasAttackLine => AttackLineSquares.Count > 0;

        protected override void CalculateMovesAndCapturesInternal()
        {
            PinnedPiece = null;
            _pinnedPieceCount = 0;
            AttackLineSquares.Clear();

            foreach (Vector2Int direction in moveVectors)
            {
                ProcessAttackLine(direction);
            }
        }

        private void ProcessAttackLine(Vector2Int direction)
        {
            Vector2Int offset = direction;
            HashSet<Square> possibleAttackLine = new();
            while (Application.isPlaying)
            {
                Square square = Game.GetSquareRel(pieceColor, currentSquare, offset);
                offset += direction;

                if (square == Game.NullSquare)
                {
                    break;
                }

                if (square.HasPiece())
                {
                    if (square.GetPieceColor() != pieceColor)
                    {
                        // Found attack line directly to king
                        if (square.GetPiece() is King)
                        {
                            AddAttackLineAndCaptureKingIfNeeded(possibleAttackLine, square);
                            break;
                        }

                        AddPinnedPiece(square);
                        if (IfFoundMoreThanOnePinnedPiece()) break;

                        possibleAttackLine.Add(square);
                        CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
                    }
                    // Found piece of same color
                    else
                    {
                        DefendSquares.Add(square);
                        break;
                    }
                }
                // Found empty square
                else
                {
                    possibleAttackLine.Add(square);
                    MoveSquares.Add(square, new MoveInfo());
                }
            }
        }

        private void AddAttackLineAndCaptureKingIfNeeded(HashSet<Square> possibleAttackLine, Square square)
        {
            AttackLineSquares = possibleAttackLine;
            if(_pinnedPieceCount == 0)
                CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
        }

        private void AddPinnedPiece(Square square)
        {
            PinnedPiece = square.GetPiece();
            _pinnedPieceCount += 1;
        }

        private bool IfFoundMoreThanOnePinnedPiece()
        {
            if(_pinnedPieceCount > 1)
            {
                PinnedPiece = null;
                return true;
            }

            return false;
        }
    }
}