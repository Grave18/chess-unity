using Chess3D.Runtime.ChessBoard;
using Chess3D.Runtime.ChessBoard.Pieces;
using Chess3D.Runtime.Sound;
using UnityEngine;

namespace Chess3D.Runtime.Logic.Moves
{
    public class SimpleMove : Turn
    {
        private readonly Game _game;
        private readonly Piece _movedPiece;
        private readonly Square _fromSquare;
        private readonly Square _toSquare;
        private readonly bool _isFirstMove;
        private readonly bool _isComposite;

        public SimpleMove(Game game, Piece movedPiece, Square fromSquare, Square toSquare, bool isFirstMove, bool isComposite = false)
        {
            _game = game;
            _movedPiece = movedPiece;
            _fromSquare = fromSquare;
            _toSquare = toSquare;
            _isFirstMove = isFirstMove;
            _isComposite = isComposite;
        }

        public override void Progress(float t)
        {
            Vector3 from = _fromSquare.transform.position;
            Vector3 to = _toSquare.transform.position;
            _movedPiece.MoveTo(from, to, t);
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
                if (_game.CurrentTurnColor == PieceColor.White)
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