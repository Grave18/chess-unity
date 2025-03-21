using System.Collections.Generic;
using ChessBoard.Pieces;
using Logic;
using UnityEngine;

namespace ChessBoard
{
    public class BeatenPieces : MonoBehaviour
    {
        [SerializeField] private Transform whitePlaces;
        [SerializeField] private Transform blackPlaces;

        private int _whiteIndex;
        private int _blackIndex;

        private readonly Stack<Piece> _beatenPieces = new();

        public static BeatenPieces Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Add(Piece piece)
        {
            switch (piece.GetPieceColor())
            {
                case PieceColor.White:
                    if(_whiteIndex >= whitePlaces.childCount) return;
                    piece.transform.position = whitePlaces.GetChild(_whiteIndex).position;
                    _whiteIndex += 1;
                    break;
                case PieceColor.Black:
                    if(_blackIndex >= blackPlaces.childCount) return;
                    piece.transform.position = blackPlaces.GetChild(_blackIndex).position;
                    _blackIndex += 1;
                    break;
            }

            _beatenPieces.Push(piece);
        }

        public Piece Remove()
        {
            Piece lastPiece;

            if (_beatenPieces.Count > 0)
            {
                lastPiece = _beatenPieces.Pop();
                switch (lastPiece.GetPieceColor())
                {
                    case PieceColor.White:
                        if(_whiteIndex <= 0) return null;
                        _whiteIndex -= 1;
                        break;
                    case PieceColor.Black:
                        if(_blackIndex <= 0) return null;
                        _blackIndex -= 1;
                        break;
                }
            }
            else
            {
                return null;
            }


            return lastPiece;
        }
    }
}
