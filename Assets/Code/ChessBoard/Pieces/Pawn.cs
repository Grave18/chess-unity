using AlgebraicNotation;
using ChessBoard.Info;
using Logic.CommandPattern;
using UnityEngine;

namespace ChessBoard.Pieces
{
    public class Pawn : Piece
    {
        [Header("Pawn")]
        [SerializeField] private Vector2Int[] movesFirstMove;
        [SerializeField] private Vector2Int[] moves;
        [SerializeField] private Vector2Int[] eat;

        protected override void CalculateMovesAndCapturesInternal()
        {
            Vector2Int[] currentMoves = IsFirstMove ? movesFirstMove : moves;

            // Calculate moves
            foreach (Vector2Int offset in currentMoves)
            {
                Square square = Game.GetSquareRel(pieceColor, currentSquare, offset);

                if (square.HasPiece() || square == Game.NullSquare)
                {
                    break;
                }

                bool is2SquaresMove = offset.y == 2;
                MoveSquares.Add(square, new MoveInfo(is2SquaresMove));
            }

            // Calculate Captures and defends
            foreach (Vector2Int offset in eat)
            {
                Square square = Game.GetSquareRel(pieceColor, currentSquare, offset);

                if (square.HasPiece())
                {
                    CalculateCapturesAndDefends(square);
                }
                else
                {
                    CalculateEnPassantCapture(square);
                }
            }
        }

        private void CalculateCapturesAndDefends(Square square)
        {
            if (square.GetPieceColor() != pieceColor)
            {
                CaptureSquares.Add(square, new CaptureInfo(square.GetPiece()));
            }
            else
            {
                DefendSquares.Add(square);
            }
        }

        private void CalculateEnPassantCapture(Square square)
        {
            EnPassantInfo enPassantInfo = Game.GetEnPassantInfo();
            if(enPassantInfo != null && square == enPassantInfo.Square)
            {
                CaptureSquares.Add(square, new CaptureInfo(enPassantInfo.Piece, NotationTurnType.EnPassant));
            }
        }
    }
}