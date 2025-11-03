using System.Collections.Generic;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic;
using UnityEngine;

namespace Chess3D.Runtime.Core.ChessBoard
{
    public class BeatenPieces : MonoBehaviour
    {
        [SerializeField] private Transform whitePlaces;
        [SerializeField] private Transform blackPlaces;

        private int _whiteIndex;
        private int _blackIndex;

        private readonly Stack<(Piece, Square)> _beatenPieces = new();

        public static BeatenPieces Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Add(Piece piece, Square pieceSquare)
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

            _beatenPieces.Push((piece, pieceSquare));
        }

        public (Piece, Square) Remove()
        {
            (Piece,Square) lastPiece;

            if (_beatenPieces.Count > 0)
            {
                lastPiece = _beatenPieces.Pop();
                switch (lastPiece.Item1.GetPieceColor())
                {
                    case PieceColor.White:
                        if(_whiteIndex <= 0) return (null, null);
                        _whiteIndex -= 1;
                        break;
                    case PieceColor.Black:
                        if(_blackIndex <= 0) return (null, null);
                        _blackIndex -= 1;
                        break;
                }
            }
            else
            {
                return (null, null);
            }

            return lastPiece;
        }

        public void Clear()
        {
            _beatenPieces.Clear();
            _whiteIndex = 0;
            _blackIndex = 0;
        }
    }
}
