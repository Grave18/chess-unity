using System.Collections.Generic;
using Chess3D.Runtime.ChessBoard.Info;
using UnityEngine;

namespace Chess3D.Runtime.ChessBoard.Pieces
{
    public class LongRange : Piece
    {
        [Header("Long Range")]
        [SerializeField] private Vector2Int[] moveVectors;

        /// Line from long range piece to opposite king
        public HashSet<Square> AttackLineSquares { get; private set; } = new();
        // Square on attack line but behind king piece
        public Square SquareBehindKing { get; private set; }

        private int _pinnedPieceCount;

        public bool HasAttackLine => AttackLineSquares != null;

        protected override void CalculateMovesAndCapturesInternal()
        {
            AttackLineSquares = null;
            SquareBehindKing = null;

            foreach (Vector2Int direction in moveVectors)
            {
                ProcessAttackLine(direction);
            }
        }

        private void ProcessAttackLine(Vector2Int direction)
        {
            Vector2Int offset = direction;
            HashSet<Square> possibleAttackLine = new();
            _pinnedPieceCount = 0;
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
                            AttackLineSquares = possibleAttackLine;
                            if (IsNoPinnedPiece())
                            {
                                CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
                                SquareBehindKing = Game.GetSquareRel(pieceColor, square, direction);
                            }
                            break;
                        }

                        // Found pinned piece
                        IncreasePinnedPieceCount();
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
                    if(IsNoPinnedPiece())
                    {
                        MoveSquares.Add(square, new MoveInfo());
                    }
                }
            }
        }

        private void IncreasePinnedPieceCount()
        {
            _pinnedPieceCount += 1;
        }

        private bool IfFoundMoreThanOnePinnedPiece()
        {
            return _pinnedPieceCount > 1;
        }

        private bool IsNoPinnedPiece()
        {
            return _pinnedPieceCount == 0;
        }
    }
}