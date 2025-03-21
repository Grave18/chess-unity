using ChessBoard;
using ChessBoard.Pieces;
using UnityEngine;
using Utils.Mathematics;

namespace Logic.Players.Moves
{
    public class Capture : Turn
    {
        private readonly Piece _movedPiece;
        private readonly Piece _beatenPiece;
        private readonly Square _fromSquare;
        private readonly Square _toSquare;
        private readonly bool _isFirstMove;

        public Capture(Piece movedPiece, Square fromSquare, Square toSquare, Piece beatenPiece, bool isFirstMove)
        {
            _movedPiece = movedPiece;
            _beatenPiece = beatenPiece;
            _fromSquare = fromSquare;
            _toSquare = toSquare;
            _isFirstMove = isFirstMove;
        }

        public override void Progress(float t)
        {
            Vector3 from = _fromSquare.transform.position;
            Vector3 to = _toSquare.transform.position;
            Vector3 pos = Vector3.Lerp(from, to, Easing.InOutCubic(t));

            Piece movedPiece = _movedPiece.GetPiece();
            movedPiece.MoveTo(pos);
        }

        public override void End()
        {
            Square beatenPieceSquare = _beatenPiece.GetSquare();

            _beatenPiece.RemoveFromBoard();
            BeatenPieces.Instance.Add(_beatenPiece, beatenPieceSquare);
            _movedPiece.SetNewSquare(_toSquare);

            if (_isFirstMove)
            {
                _movedPiece.IsFirstMove = false;
            }
        }

        public override void EndUndo()
        {
            (Piece beatenPiece, Square beatenPieceSquare) = BeatenPieces.Instance.Remove();

            _movedPiece.SetNewSquare(_fromSquare);
            beatenPiece.AddToBoard(beatenPieceSquare);

            if (_isFirstMove)
            {
                _movedPiece.IsFirstMove = true;
            }
        }
    }
}