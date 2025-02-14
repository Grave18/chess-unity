using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessBoard.Pieces;
using EditorCools;
using Logic;
using Ui.Promotion;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChessBoard.Builder
{
    public class Board : MonoBehaviour
    {
        private const int Width = 8;
        private const int Height = 8;

        [FormerlySerializedAs("gameManager")] [Header("References")] [SerializeField]
        private Game game;

        [SerializeField] private PromotionPanel promotionPanel;

        [SerializeField] private Transform whitePiecesParent;
        [SerializeField] private Transform blackPiecesParent;
        [SerializeField] private GameObject[] piecePrefabs;
        [SerializeField] private BoardPreset boardPreset;

        private readonly Dictionary<Square, GameObject> _whitePairs = new();
        private readonly Dictionary<Square, GameObject> _blackPairs = new();

        private TaskCompletionSource<PieceType> _pieceTypeCompletionSource = new();

        public GameObject[] PiecePrefabs => piecePrefabs;

        public async Task<(Piece, PieceType)> GetPieceFromSelectorAsync(PieceColor pieceColor, Square square)
        {
            promotionPanel.Show(pieceColor);
            PieceType pieceType = await _pieceTypeCompletionSource.Task;
            promotionPanel.Hide();

            Piece piece = GetPiece(pieceType, pieceColor, square);
            return (piece, pieceType);
        }

        public void Select(PieceType pieceType)
        {
            _pieceTypeCompletionSource.SetResult(pieceType);
            _pieceTypeCompletionSource = new TaskCompletionSource<PieceType>();
        }

        public Piece GetPiece(PieceType pieceType, PieceColor pieceColor, Square square)
        {
            Transform piecesParent = pieceColor == PieceColor.White
                ? whitePiecesParent
                : blackPiecesParent;

            GameObject piecePrefab = pieceColor == PieceColor.White
                ? pieceType switch
                {
                    // White
                    PieceType.Queen => piecePrefabs[10],
                    PieceType.Rook => piecePrefabs[11],
                    PieceType.Bishop => piecePrefabs[6],
                    PieceType.Knight => piecePrefabs[8],
                    PieceType.Pawn => piecePrefabs[9],
                    _ => null,
                }
                : pieceType switch
                {
                    // Black
                    PieceType.Queen => piecePrefabs[4],
                    PieceType.Rook => piecePrefabs[5],
                    PieceType.Bishop => piecePrefabs[0],
                    PieceType.Knight => piecePrefabs[2],
                    PieceType.Pawn => piecePrefabs[3],
                    _ => null,
                };

            InstantiatePiece(piecesParent, piecePrefab, square, out Piece piece);
            return piece;
        }

        public void BuildBoard(out PieceColor turnColor)
        {
            string text = boardPreset.Preset;
            turnColor = boardPreset.TurnColor;

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
                        _blackPairs[game.Squares[iSquares]] = piecePrefabs[0]; // B_Bishop
                        break;
                    case 'k':
                        _blackPairs[game.Squares[iSquares]] = piecePrefabs[1]; // B_King
                        break;
                    case 'n':
                        _blackPairs[game.Squares[iSquares]] = piecePrefabs[2]; // B_Knight
                        break;
                    case 'p':
                        _blackPairs[game.Squares[iSquares]] = piecePrefabs[3]; // B_Pawn
                        break;
                    case 'q':
                        _blackPairs[game.Squares[iSquares]] = piecePrefabs[4]; // B_Queen
                        break;
                    case 'r':
                        _blackPairs[game.Squares[iSquares]] = piecePrefabs[5]; // B_Rook
                        break;
                    case 'B':
                        _whitePairs[game.Squares[iSquares]] = piecePrefabs[6]; // W_Bishop
                        break;
                    case 'K':
                        _whitePairs[game.Squares[iSquares]] = piecePrefabs[7]; // W_King
                        break;
                    case 'N':
                        _whitePairs[game.Squares[iSquares]] = piecePrefabs[8]; // W_Knight
                        break;
                    case 'P':
                        _whitePairs[game.Squares[iSquares]] = piecePrefabs[9]; // W_Pawn
                        break;
                    case 'Q':
                        _whitePairs[game.Squares[iSquares]] = piecePrefabs[10]; // W_Queen
                        break;
                    case 'R':
                        _whitePairs[game.Squares[iSquares]] = piecePrefabs[11]; // W_Rook
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

            game.FindAllPieces();
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
                InstantiatePiece(piecesParent, piece, square, out _);
            }
        }

        private void InstantiatePiece(Transform piecesParent, GameObject piecePrefab, Square square, out Piece piece)
        {
            GameObject pieceInstance = Instantiate(piecePrefab, square.transform.position,
                piecePrefab.transform.rotation, piecesParent);
            piece = pieceInstance.GetComponent<Piece>();
            piece.GetSectionAndAlign(game);
        }

        [Button(name: "Build Board", space: 10f)]
        [ContextMenu("Build Board")]
        private void GameManagerRestart()
        {
            game.Restart();
        }

        [Button(space: 10f)]
        [ContextMenu("Destroy All Pieces")]
        private void DestroyAllPieces()
        {
            DestroyColorPieces(whitePiecesParent);
            DestroyColorPieces(blackPiecesParent);
            _whitePairs.Clear();
            _blackPairs.Clear();

            game.ClearPieces();
        }

        private static void DestroyColorPieces(Transform parent)
        {
            Transform[] pieces = parent.Cast<Transform>().ToArray();
            foreach (Transform piece in pieces)
            {
                DestroyImmediate(piece.gameObject);
            }
        }
    }
}