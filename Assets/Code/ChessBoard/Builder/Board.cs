using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessBoard.Pieces;
using EditorCools;
using Logic;
using Ui.Promotion;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        [SerializeField] private AssetReferenceGameObject[] piecePrefabs;
        [SerializeField] private BoardPreset boardPreset;

        private readonly Dictionary<Square, AssetReferenceGameObject> _whitePairs = new();
        private readonly Dictionary<Square, AssetReferenceGameObject> _blackPairs = new();

        private TaskCompletionSource<PieceType> _pieceTypeCompletionSource = new();

        public async Task<(Piece, PieceType)> GetPieceFromSelectorAsync(PieceColor pieceColor, Square square)
        {
            promotionPanel.Show(pieceColor);
            PieceType pieceType = await _pieceTypeCompletionSource.Task;
            promotionPanel.Hide();

            Piece piece = await GetPieceAsync(pieceType, pieceColor, square);
            return (piece, pieceType);
        }

        public void Select(PieceType pieceType)
        {
            _pieceTypeCompletionSource.SetResult(pieceType);
            _pieceTypeCompletionSource = new TaskCompletionSource<PieceType>();
        }

        public async Task<Piece> GetPieceAsync(PieceType pieceType, PieceColor pieceColor, Square square)
        {
            Transform piecesParent
                = pieceColor == PieceColor.White
                ? whitePiecesParent
                : blackPiecesParent;

            AssetReferenceGameObject piecePrefab
                = pieceColor == PieceColor.White
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

            Piece piece = await InstantiatePieceAsync(piecesParent, piecePrefab, square);
            return piece;
        }



        [Button(space: 10f)]
        [ContextMenu("Destroy All Pieces")]
        public void DestroyAllPieces()
        {
            DestroyColorPieces(whitePiecesParent);
            DestroyColorPieces(blackPiecesParent);
            _whitePairs.Clear();
            _blackPairs.Clear();

            game.ClearPieces();
        }

        [Button(name: "Build Board", space: 10f)]
        [ContextMenu("Build Board")]
        public async Task BuildBoardAsync()
        {
            await BuildBoardAsync(boardPreset);
        }

        public async Task BuildBoardAsync(BoardPreset preset)
        {
            boardPreset = preset;
            string text = preset.Preset;

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

            await InstantiatePieces();

            game.FindAllPieces();
            game.SetTurn(preset.TurnColor);
        }

        private static bool IsPresetNotValid(string text)
        {
            return text.Length != Width * Height;
        }

        private static string ClearNewLineCharacters(string text)
        {
            return text.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
        }

        private async Task InstantiatePieces()
        {
            Task taskWhite = InstantiatePiecesAsync(_whitePairs, whitePiecesParent);
            Task taskBlack = InstantiatePiecesAsync(_blackPairs, blackPiecesParent);

            await Task.WhenAll(taskWhite, taskBlack);
        }

        private async Task InstantiatePiecesAsync(Dictionary<Square, AssetReferenceGameObject> pairs, Transform piecesParent)
        {
            var tasks = new List<Task<Piece>>();
            foreach (var (square, piece) in pairs)
            {
                tasks.Add(InstantiatePieceAsync(piecesParent, piece, square));
            }

            await Task.WhenAll(tasks);
        }

        private async Task<Piece> InstantiatePieceAsync(Transform piecesParent, AssetReferenceGameObject assetReference, Square square)
        {
            if (assetReference == null)
            {
                return null;
            }

            GameObject pieceInstance = await assetReference.InstantiateAsync(square.transform.position, Quaternion.identity, piecesParent).Task;

            var piece = pieceInstance.GetComponent<Piece>();
            RotateBlack(piece);
            piece.Init(game);

            return piece;
        }

        private static void RotateBlack(Piece piece)
        {
            if (piece.GetPieceColor() == PieceColor.Black)
            {
                piece.transform.Rotate(0f, 180f, 0f);
            }
        }

        private static void DestroyColorPieces(Transform parent)
        {
            Transform[] pieces = parent.Cast<Transform>().ToArray();
            foreach (Transform piece in pieces)
            {
                if (Application.isPlaying)
                {
                    if (!Addressables.ReleaseInstance(piece.gameObject))
                    {
                        Destroy(piece.gameObject);
                    }
                }
                else
                {
                    DestroyImmediate(piece.gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            ReleaseAssets();
        }

        private void ReleaseAssets()
        {
            var assetReferences = _blackPairs.Values.Concat(_whitePairs.Values);
            foreach (AssetReferenceGameObject assetReference in assetReferences)
            {
                if (assetReference != null && assetReference.IsValid())
                {
                    assetReference.ReleaseAsset();
                }
            }
        }
    }
}