using System;
using System.Collections.Generic;
using System.Linq;
using Board.Pieces;
using Logic;
using UnityEngine;

namespace Board.Builder
{
    public class BoardBuilder : MonoBehaviour
    {
        private const int Width = 8;
        private const int Height = 8;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private Transform whitePiecesParent;
        [SerializeField] private Transform blackPiecesParent;
        [SerializeField] private GameObject[] piecePrefabs;
        [SerializeField] private TextAsset boardPreset;

        private readonly Dictionary<Square, GameObject> _whitePairs = new();
        private readonly Dictionary<Square, GameObject> _blackPairs = new();

        [ContextMenu("Build Board")]
        public void BuildBoard()
        {
            DestroyAllPieces();

            string text = boardPreset.text;

            for (int x = 0, y = 0; y < Height;)
            {
                // Add y because of \n symbols
                int iText = x + y * Width + y;
                // Must invert y (Height - 1 - y)
                int iSquares = Height - 1 - y + x * Width;

                if (text[iText] == '\n')
                {
                    x = 0;
                    y += 1;

                    continue;
                }

                if (text[iText] == ' ')
                {
                    x += 1;
                    continue;
                }

                switch (text[iText])
                {
                    case '*':
                        break;
                    case 'b':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[0]; // B_Bishop
                        break;
                    case 'k':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[1]; // B_King
                        break;
                    case 'h':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[2]; // B_Knight
                        break;
                    case 'p':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[3]; // B_Pawn
                        break;
                    case 'q':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[4]; // B_Queen
                        break;
                    case 'r':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[5]; // B_Rook
                        break;
                    case 'B':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[6]; // W_Bishop
                        break;
                    case 'K':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[7]; // W_King
                        break;
                    case 'H':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[8]; // W_Knight
                        break;
                    case 'P':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[9]; // W_Pawn
                        break;
                    case 'Q':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[10]; // W_Queen
                        break;
                    case 'R':
                        _whitePairs[gameManager.Squares[iSquares]] = piecePrefabs[11]; // W_Rook
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                x += 1;
            }

            InstantiatePieces();

            gameManager.FindAllPieces();
        }

        private void InstantiatePieces()
        {
            InstantiatePieces(_whitePairs, whitePiecesParent);
            InstantiatePieces(_blackPairs, blackPiecesParent);
        }

        private static void InstantiatePieces(Dictionary<Square, GameObject> pairs, Transform piecesParent)
        {
            foreach (var (square, piece) in pairs)
            {
                var pieceInstance = Instantiate(piece, square.transform.position, piece.transform.rotation, piecesParent);
                var pieceComponent = pieceInstance.GetComponent<Piece>();
                pieceComponent.GetSectionAndAlign();
            }
        }

        [ContextMenu("Destroy All Pieces")]
        public void DestroyAllPieces()
        {
            DestroyAllPieces(whitePiecesParent);
            DestroyAllPieces(blackPiecesParent);
            _whitePairs.Clear();
            _whitePairs.Clear();

            gameManager.ClearSquares();
        }

        private static void DestroyAllPieces(Transform parent)
        {
            var pieces = parent.Cast<Transform>().ToArray();
            foreach (Transform piece in pieces)
            {
                DestroyImmediate(piece.gameObject);
            }
        }
    }
}
