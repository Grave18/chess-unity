using ChessBoard.Pieces;
using UnityEngine;

namespace ChessBoard
{
    public class BeatenPiecesPlace : MonoBehaviour
    {
        [SerializeField] private Transform places;
        private int _index;

        public void Add(Piece piece)
        {
            if(_index >= places.childCount) return;

            piece.transform.position = places.GetChild(_index).position;
            _index += 1;
        }

        public void Remove(Piece piece, Vector3 oldPiecePosition)
        {
            if(_index <= 0) return;

            piece.transform.position = oldPiecePosition;
            _index -= 1;
        }
    }
}
