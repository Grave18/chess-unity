using UnityEngine;

namespace Board.Pieces
{
    public class Pawn : Piece
    {
        [Header("Pawn")]
        public Vector2Int[] MovesFirstMove;
        public Vector2Int[] Moves;
        public Vector2Int[] Eat;

        public override void CalculateMovesAndCaptures()
        {
            MoveSquares.Clear();
            CaptureSquares.Clear();

            // Calculate Move
            var moves = IsFirstMove ? MovesFirstMove : Moves;
            foreach (Vector2Int offset in moves)
            {
                var square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                if (square.HasPiece() || square == gameManager.NullSquare)
                {
                    break;
                }

                MoveSquares.Add(square);
            }

            // Calculate Captures
            foreach (Vector2Int offset in Eat)
            {
                var square = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                if (square.HasPiece() && square.GetPieceColor() != pieceColor)
                {
                    CaptureSquares.Add(square);
                }
            }
        }

        protected override void CalculateUnderAttackSquaresInternal()
        {
            UnderAttackSquares.Clear();
            foreach (Vector2Int offset in Eat)
            {
                var underAttackSquare = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                // Has Piece
                if (underAttackSquare.HasPiece())
                {
                    if (underAttackSquare.GetPieceColor() == pieceColor)
                    {
                        continue;
                    }

                    UnderAttackSquares.Add(underAttackSquare);
                    continue;
                }

                // Off board
                if (underAttackSquare == gameManager.NullSquare)
                {
                    continue;
                }

                // Empty Square
                UnderAttackSquares.Add(underAttackSquare);
            }
        }

        protected override bool CanEatAtInternal(Square square)
        {
            // Early exit if section has no piece or piece of the same color
            if (!square.HasPiece() || square.GetPieceColor() == pieceColor)
            {
                return false;
            }

            CalculateUnderAttackSquares();

            foreach (var underAttackSquare in UnderAttackSquares)
            {
                if (square == underAttackSquare)
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool CanMoveToInternal(Square square)
        {
            if (square.HasPiece())
            {
                return false;
            }

            Vector2Int[] move;
            if (IsFirstMove)
            {
                move = MovesFirstMove;

                // If move two sections check if passing section is empty
                var passedSection = gameManager.GetSquareRel(pieceColor, currentSquare, move[0]);
                if (passedSection.HasPiece())
                {
                    return false;
                }
            }
            else
            {
                move = Moves;
            }

            foreach (Vector2Int offset in move)
            {
                var possibleSection = gameManager.GetSquareRel(pieceColor, currentSquare, offset);

                if (possibleSection == square)
                {
                    return true;
                }
            }

            return false;
        }
    }
}