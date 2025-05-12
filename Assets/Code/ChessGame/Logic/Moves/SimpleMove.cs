using ChessGame.ChessBoard;
using ChessGame.ChessBoard.Pieces;
using UnityEngine;
using Utils.Mathematics;

namespace ChessGame.Logic.Moves
{
    public class SimpleMove : Turn
    {
        private readonly Piece _movedPiece;
        private readonly Square _fromSquare;
        private readonly Square _toSquare;
        private readonly bool _isFirstMove;

        public SimpleMove(Piece movedPiece, Square fromSquare, Square toSquare, bool isFirstMove)
        {
            _movedPiece = movedPiece;
            _fromSquare = fromSquare;
            _toSquare = toSquare;
            _isFirstMove = isFirstMove;
        }

        public override void Progress(float t)
        {
            Vector3 from = _fromSquare.transform.position;
            Vector3 to = _toSquare.transform.position;
            Vector3 pos = Vector3.Lerp(from, to, Easing.InOutCubic(t));

            _movedPiece.MoveTo(pos);
        }

        public override void End()
        {
            _movedPiece.SetNewSquare(_toSquare);

            if (_isFirstMove)
            {
                _movedPiece.IsFirstMove = false;
            }
        }

        public override void EndUndo()
        {
            _movedPiece.SetNewSquare(_fromSquare);

            if (_isFirstMove)
            {
                _movedPiece.IsFirstMove = true;
            }
        }
    }
}