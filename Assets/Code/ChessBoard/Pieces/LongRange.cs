using System.Collections.Generic;
using ChessBoard.Info;
using UnityEngine;

namespace ChessBoard.Pieces
{
    public class LongRange : Piece
    {
        [Header("Long Range")]
        [SerializeField] private Vector2Int[] moveVectors;

        /// Line from long range piece to opposite king
        public HashSet<Square> AttackLineSquares { get; private set; } = new();

        private int _pinnedPieceCount;

        public bool HasAttackLine => AttackLineSquares.Count > 0;

        protected override void CalculateMovesAndCapturesInternal()
        {
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
                            AddAttackLineAndCaptureKingIfNeeded(possibleAttackLine, square);
                            break;
                        }

                        // Found pinned piece
                        AddPinnedPieceCount();
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
                    if(IsNeededToAddMove())
                    {
                        MoveSquares.Add(square, new MoveInfo());
                    }
                }
            }
        }

        private void AddAttackLineAndCaptureKingIfNeeded(HashSet<Square> possibleAttackLine, Square square)
        {
            AttackLineSquares = possibleAttackLine;
            if(_pinnedPieceCount == 0)
                CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
        }

        private void AddPinnedPieceCount()
        {
            _pinnedPieceCount += 1;
        }

        private bool IfFoundMoreThanOnePinnedPiece()
        {
            return _pinnedPieceCount > 1;
        }

        private bool IsNeededToAddMove()
        {
            return _pinnedPieceCount == 0;
        }
    }
}