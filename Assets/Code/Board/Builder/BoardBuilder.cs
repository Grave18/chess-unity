using System.Collections.Generic;
using System.Linq;
using Board.Pieces;
using EditorCools;
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
        [SerializeField] private BoardPreset boardPreset;

        private readonly Dictionary<Square, GameObject> _whitePairs = new();
        private readonly Dictionary<Square, GameObject> _blackPairs = new();

        public void BuildBoard()
        {
            string text = boardPreset.Preset;

            text = ClearNewLineCharacters(text);

            if (IsPresetNotValid(text))
            {
                Debug.LogError("Invalid board preset");
                return;
            }

            DestroyAllPieces();

            for (int x = 0, y = 0; y < Height;)
            {
                int iText = x + y * Width;
                // Must invert y (Height - 1 - y) also x and y are swapped
                int iSquares = Height - 1 - y + x * Width;

                switch (text[iText])
                {
                    // Set prefabs for each square
                    case '*':
                    case ' ':
                        break;
                    case 'b':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[0]; // B_Bishop
                        break;
                    case 'k':
                        _blackPairs[gameManager.Squares[iSquares]] = piecePrefabs[1]; // B_King
                        break;
                    case 'n':
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
                    case 'N':
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
                        Debug.LogError($"{text[iText]} is not a valid character");
                        return;
                }

                x += 1;

                if (x == Width)
                {
                    x = 0;
                    y += 1;
                }
            }

            InstantiatePieces();

            gameManager.FindAllPieces();
        }

        private static bool IsPresetNotValid(string text)
        {
            return text.Length != Width * Height;
        }

        private static string ClearNewLineCharacters(string text)
        {
            return text.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
        }

        private void InstantiatePieces()
        {
            InstantiatePieces(_whitePairs, whitePiecesParent);
            InstantiatePieces(_blackPairs, blackPiecesParent);
        }

        private void InstantiatePieces(Dictionary<Square, GameObject> pairs, Transform piecesParent)
        {
            foreach (var (square, piece) in pairs)
            {
                var pieceInstance = Instantiate(piece, square.transform.position, piece.transform.rotation, piecesParent);
                var pieceComponent = pieceInstance.GetComponent<Piece>();
                pieceComponent.GetSectionAndAlign(gameManager);
            }
        }

        [Button(name: "Build Board", space: 10f)]
        [ContextMenu("Build Board")]
        private void GameManagerRestart()
        {
            gameManager.Restart();
        }

        [Button(space: 10f)]
        [ContextMenu("Destroy All Pieces")]
        private void DestroyAllPieces()
        {
            DestroyAllPieces(whitePiecesParent);
            DestroyAllPieces(blackPiecesParent);
            _whitePairs.Clear();
            _blackPairs.Clear();

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
