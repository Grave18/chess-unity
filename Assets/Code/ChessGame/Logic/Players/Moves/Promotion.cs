using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Pieces;
using UnityEngine;

namespace ChessGame.Logic.Players.Moves
{
    public class Promotion
    {
        private readonly Piece _piece;
        private readonly Square _toSquare;
        private readonly Square _fromSquare;
        private readonly Piece _otherPiece;

        public Promotion(Piece piece, Square fromSquare, Square toSquare, Piece otherPiece)
        {
            _piece = piece;
            _fromSquare = fromSquare;
            _toSquare = toSquare;
            _otherPiece = otherPiece;
        }

        public void End()
        {
            _piece.RemoveFromBoard();
            _piece.gameObject.SetActive(false);
            _otherPiece.AddToBoard(_toSquare);
        }

        public void EndUndo()
        {
            _piece.RemoveFromBoard();
            Object.Destroy(_piece.gameObject);
            _otherPiece.AddToBoard(_fromSquare);
        }
    }
}