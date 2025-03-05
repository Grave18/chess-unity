using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetsAndResources;
using ChessBoard.Pieces;
using EditorCools;
using Logic;
using Logic.Players;
using Ui.Promotion;
using UnityEngine;

namespace ChessBoard
{
    public class Board : MonoBehaviour
    {
        public const int Width = 8;
        public const int Height = 8;

        [Header("References")]
        [SerializeField] private Competitors competitors;
        [SerializeField] private Transform piecesParent;
        [SerializeField] private Transform squaresParent;
        [SerializeField] private Square nullSquare;

        [Header("Ui")]
        [SerializeField] private PromotionPanel promotionPanel;

        // This is piece references what is on board in current game
        public HashSet<Piece> WhitePieces { get; } = new();
        public HashSet<Piece> BlackPieces { get; } = new();
        public List<Square> Squares { get; } = new();
        private Dictionary<string, Square> SquaresHash { get; } = new();

        // Initialized
        private Game _game;
        private GameObject[] _prefabs;
        private BoardPreset _boardPreset;

        private readonly Dictionary<Square, GameObject> _piecePairs = new();
        private GameObject _boardInstance;

        public Square NullSquare => nullSquare;

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
            PieceType pieceType = await competitors.RequestPromotedPiece();
            promotionPanel.Hide();

            Piece piece = GetPiece(pieceType, pieceColor, square);
            return (piece, pieceType);
        }

        public void Select(PieceType pieceType)
        {
            competitors.SelectPromotedPiece(pieceType);
        }

        public void AddPiece(Piece piece)
        {
            if (piece.GetPieceColor() == PieceColor.White)
            {
                WhitePieces.Add(piece);
            }
            else
            {
                BlackPieces.Add(piece);
            }
        }

        public void RemovePiece(Piece piece)
        {
            if (piece.GetPieceColor() == PieceColor.White)
            {
                WhitePieces.Remove(piece);
            }
            else
            {
                BlackPieces.Remove(piece);
            }
        }

        /// <summary>
        /// Get square by address on the boadrd
        /// </summary>
        /// <param name="uciSquare"> "c1", "d3" ...</param>
        /// <returns> Square by listed address </returns>
        public Square GetSquare(string uciSquare)
        {
            return SquaresHash.TryGetValue(uciSquare, out Square moveFromSquare)
                ? moveFromSquare
                : NullSquare;
        }

        [Button(space: 10f)]
        public void Build()
        {
            string preset = GetParsedPreset();

            DestroyBoardAndPieces();
            SetupSquares();
            LoadPiecesFromPreset(preset);
            InstantiatePieces();
            InstantiateBoard();
        }

        private string GetParsedPreset()
        {
            string preset = _boardPreset.Preset;

            preset = ClearNewLineCharacters(preset);

            if (IsPresetNotValid(preset))
            {
                Debug.LogError("Invalid board preset");
            }

            return preset;
        }

        private static string ClearNewLineCharacters(string text)
        {
            return text.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
        }

        private static bool IsPresetNotValid(string text)
        {
            return text.Length != Width * Height;
        }

        [Button(space: 10f)]
        public void DestroyBoardAndPieces()
        {
            DestroyPieces();
            DestroyBoard();
            _piecePairs.Clear();
            _boardInstance = null;
            WhitePieces.Clear();
            BlackPieces.Clear();
        }

        private void LoadPiecesFromPreset(string preset)
        {
            for (int x = 0, y = 0; y < Height;)
            {
                int indexOfSymbol = x + y * Width;
                // Must invert y (Height - 1 - y) also x and y are swapped
                int indexOfSquare = Height - 1 - y + x * Width;

                Square square = Squares[indexOfSquare];
                switch (preset[indexOfSymbol])
                {
                    // Set prefabs for each square
                    case '*':
                    case ' ':
                        break;
                    case 'b': _piecePairs[square] = _prefabs[0];  break; // B_Bishop
                    case 'k': _piecePairs[square] = _prefabs[1];  break; // B_King
                    case 'n': _piecePairs[square] = _prefabs[2];  break; // B_Knight
                    case 'p': _piecePairs[square] = _prefabs[3];  break; // B_Pawn
                    case 'q': _piecePairs[square] = _prefabs[4];  break; // B_Queen
                    case 'r': _piecePairs[square] = _prefabs[5];  break; // B_Rook
                    case 'B': _piecePairs[square] = _prefabs[6];  break; // W_Bishop
                    case 'K': _piecePairs[square] = _prefabs[7];  break; // W_King
                    case 'N': _piecePairs[square] = _prefabs[8];  break; // W_Knight
                    case 'P': _piecePairs[square] = _prefabs[9];  break; // W_Pawn
                    case 'Q': _piecePairs[square] = _prefabs[10]; break; // W_Queen
                    case 'R': _piecePairs[square] = _prefabs[11]; break; // W_Rook
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

        private void InstantiatePieces()
        {
            foreach (var (square, piece) in _piecePairs)
            {
                Piece pieceInstance = InstantiatePiece(piece, square);
                AddPiece(pieceInstance);
            }
        }

        private Piece InstantiatePiece(GameObject piecePrefab, Square square)
        {
            if (piecePrefab == null)
            {
                return null;
            }

            GameObject pieceInstance = Instantiate(piecePrefab, square.transform.position, piecePrefab.transform.rotation, piecesParent);
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

        private void SetupSquares()
        {
            FindAllSquares();
            SetFilesAndRanks();
            HashSquares();
        }

        private void FindAllSquares()
        {
            // Get all squares components form parent tree
            foreach (Transform squareTransform in squaresParent)
            {
                Squares.Add(squareTransform.GetComponent<Square>());
            }
        }

        private void SetFilesAndRanks()
        {
            // Set files and ranks
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = y + x * Width;

                    Square square = Squares[index];
                    square.X = x;
                    square.Y = y;

                    square.File = $"{(char)(x + 'a')}";
                    square.Rank = $"{y + 1}";
                    square.Address = $"{square.File}{square.Rank}";
                }
            }
        }

        private void HashSquares()
        {
            // Hash files and ranks -> squares
            foreach (Square square in Squares)
            {
                SquaresHash[square.Address] = square;
            }
        }

        private void DestroyPieces()
        {
            Transform[] pieces = piecesParent.Cast<Transform>().ToArray();
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