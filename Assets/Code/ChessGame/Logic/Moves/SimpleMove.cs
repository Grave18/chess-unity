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
        private readonly bool _isComposite;

        public SimpleMove(Piece movedPiece, Square fromSquare, Square toSquare, bool isFirstMove, bool isComposite = false)
        {
            _movedPiece = movedPiece;
            _fromSquare = fromSquare;
            _toSquare = toSquare;
            _isFirstMove = isFirstMove;
            _isComposite = isComposite;
        }

        public override void Progress(float t, bool isUndo = false)
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

        public override void PlaySound()
        {
            if (!_isComposite)
            {
                if (Game.Instance.CurrentTurnColor == PieceColor.White)
                {
                    EffectsPlayer.Instance.PlayMoveSound();
                }
                else
                {
                    EffectsPlayer.Instance.PlayMoveOpponentSound();
                }
            }
        }
    }
}