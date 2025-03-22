using ChessBoard;
using ChessBoard.Pieces;
using UnityEngine;

namespace Logic.Players.Moves
{
    internal class MovePromotion : Turn
    {
        private readonly Piece _piece;
        private readonly Square _fromSquare;
        private readonly Square _toSquare;
        private readonly Piece _otherPiece;
        private readonly Turn _simpleMove;

        public MovePromotion(Piece piece, Square fromSquare, Square toSquare, Piece otherPiece)
        {
            _piece = piece;
            _fromSquare = fromSquare;
            _toSquare = toSquare;
            _otherPiece = otherPiece;
            _simpleMove = new SimpleMove(piece, fromSquare, toSquare, isFirstMove: false);
        }

        public override void Progress(float t)
        {
            _simpleMove.Progress(t);
        }

        public override void End()
        {
            // _simpleMove.End();

            _piece.RemoveFromBoard();
            _piece.gameObject.SetActive(false);
            _otherPiece.AddToBoard(_toSquare);
        }

        public override void EndUndo()
        {
            // _simpleMove.EndUndo();

            _piece.RemoveFromBoard();
            Object.Destroy(_piece.gameObject);
            _otherPiece.AddToBoard(_fromSquare);
        }
    }
}