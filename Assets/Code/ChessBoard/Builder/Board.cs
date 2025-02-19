using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessBoard.Pieces;
using EditorCools;
using Logic;
using Ui.Promotion;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ChessBoard.Builder
{
    public class Board : MonoBehaviour
    {
        private const int Width = 8;
        private const int Height = 8;

        [Header("References")]
        [SerializeField] private Transform prefabsParent;

        [Header("Ui")]
        [SerializeField] private PromotionPanel promotionPanel;

        private Game _game;
        private GameObject[] _prefabs;
        private readonly Dictionary<Square, GameObject> _piecePairs = new();

        private GameObject _boardInstance;
        private BoardPreset _boardPreset;

        // Selected in Ui
        private TaskCompletionSource<PieceType> _pieceTypeCompletionSource = new();

        public void Init(Game game, BoardPreset boardPreset, GameObject[] prefabs)
        {
            _game = game;
            _prefabs = prefabs;
            _boardPreset = boardPreset;
            Build();
        }

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

        [Button(space: 10f)]
        public void Build()
        {
            string preset = _boardPreset.Preset;

            preset = ClearNewLineCharacters(preset);

            if (IsPresetNotValid(preset))
            {
                Debug.LogError("Invalid board preset");
                return;
            }

            DestroyBoardAndPieces();
            LoadPreset(preset);
            InstantiatePieces();
            InstantiateBoard();
            _game.FindAllPieces();
        }

        private void LoadPreset(string preset)
        {
            for (int x = 0, y = 0; y < Height;)
            {
                int indexOfSymbol = x + y * Width;
                // Must invert y (Height - 1 - y) also x and y are swapped
                int indexOfSquare = Height - 1 - y + x * Width;

                switch (preset[indexOfSymbol])
                {
                    // Set prefabs for each square
                    case '*':
                    case ' ':
                        break;
                    case 'b': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[0];  break; // B_Bishop
                    case 'k': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[1];  break; // B_King
                    case 'n': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[2];  break; // B_Knight
                    case 'p': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[3];  break; // B_Pawn
                    case 'q': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[4];  break; // B_Queen
                    case 'r': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[5];  break; // B_Rook
                    case 'B': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[6];  break; // W_Bishop
                    case 'K': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[7];  break; // W_King
                    case 'N': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[8];  break; // W_Knight
                    case 'P': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[9];  break; // W_Pawn
                    case 'Q': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[10]; break; // W_Queen
                    case 'R': _piecePairs[_game.Squares[indexOfSquare]] = _prefabs[11]; break; // W_Rook
                    default:
                        Debug.LogError($"{preset[indexOfSymbol]} is not a valid character");
                        break;
                }

                x += 1;

                if (x == Width)
                {
                    x = 0;
                    y += 1;
                }
            }
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
            foreach (var (square, piece) in _piecePairs)
            {
                InstantiatePiece(piece, square);
            }
        }

        private Piece InstantiatePiece(GameObject piecePrefab, Square square)
        {
            if (piecePrefab == null)
            {
                return null;
            }

            GameObject pieceInstance = Instantiate(piecePrefab, square.transform.position, piecePrefab.transform.rotation, prefabsParent);
            var piece = pieceInstance.GetComponent<Piece>();

            piece.Init(_game);

            return piece;
        }

        private void InstantiateBoard()
        {
            _boardInstance = Instantiate(_prefabs[12], transform);
        }

        private Piece GetPiece(PieceType pieceType, PieceColor pieceColor, Square square)
        {
            GameObject piecePrefab
                = pieceColor == PieceColor.White
                    ? pieceType switch
                    {
                        // White
                        PieceType.Queen => _prefabs[10],
                        PieceType.Rook => _prefabs[11],
                        PieceType.Bishop => _prefabs[6],
                        PieceType.Knight => _prefabs[8],
                        PieceType.Pawn => _prefabs[9],
                        _ => null,
                    }
                    : pieceType switch
                    {
                        // Black
                        PieceType.Queen => _prefabs[4],
                        PieceType.Rook => _prefabs[5],
                        PieceType.Bishop => _prefabs[0],
                        PieceType.Knight => _prefabs[2],
                        PieceType.Pawn => _prefabs[3],
                        _ => null,
                    };

            Piece piece = InstantiatePiece(piecePrefab, square);
            return piece;
        }

        [Button(space: 10f)]
        public void DestroyBoardAndPieces()
        {
            DestroyPieces();
            DestroyBoard();
            _piecePairs.Clear();
            _boardInstance = null;
            _game.ClearPieces();
        }

        private void DestroyPieces()
        {
            Transform[] pieces = prefabsParent.Cast<Transform>().ToArray();
            foreach (Transform piece in pieces)
            {
                Destroy(piece.gameObject);
            }
        }

        private void DestroyBoard()
        {
            if (_boardInstance == null)
            {
                return;
            }

            Destroy(_boardInstance.gameObject);
        }
    }
}