using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetsAndResources;
using ChessBoard.Info;
using ChessBoard.Pieces;
using EditorCools;
using Logic;
using Logic.MovesBuffer;
using Logic.Players;
using Ui.Promotion;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

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

        /// This is white piece references what is on board in current game
        public HashSet<Piece> WhitePieces { get; } = new();
        /// This is black piece references what is on board in current game
        public HashSet<Piece> BlackPieces { get; } = new();
        public List<Square> Squares { get; } = new();
        private Dictionary<string, Square> SquaresHash { get; } = new();

        // Initialized
        private Game _game;
        private Buffer _commandBuffer;
        private GameObject[] _prefabs;
        private ParsedPreset _boardPreset;
        private PieceColor _turnColor;

        private GameObject _boardInstance;

        public Square NullSquare => nullSquare;

        public void Init(Game game, Buffer commandBuffer, ParsedPreset boardPreset, GameObject[] prefabs,
            PieceColor turnColor)
        {
            _game = game;
            _commandBuffer = commandBuffer;
            _prefabs = prefabs;
            _boardPreset = boardPreset;
            _turnColor = turnColor;
        }

        /// Gets a piece from the promotion selector if player is human. Or from Ai if player is computer
        public async Task<(Piece, PieceType)> GetPieceFromSelectorAsync(PieceColor pieceColor, Square square)
        {
            promotionPanel.Show(pieceColor);
            PieceType pieceType = await competitors.RequestPromotedPiece();
            promotionPanel.Hide();

            Piece piece = CreatePiece(pieceType, pieceColor, square);
            return (piece, pieceType);
        }

        /// Select after some amount of time, for use by real player
        public void Select(PieceType pieceType)
        {
            competitors.SelectPromotedPiece(pieceType);
        }

        /// Add piece to the board. Add it to the list of the corresponding color.
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

        /// Remove piece from the board. Remove it from the list of the corresponding color.
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

        /// Get square by address on the board. example: "c1", "d3" ...
        public Square GetSquare(string squareAddress)
        {
            return SquaresHash.TryGetValue(squareAddress, out Square moveFromSquare)
                ? moveFromSquare
                : NullSquare;
        }

        /// Get square by coordinates on the board. x: (0-7) left to right. y: (0-7) bottom to top
        public Square GetSquare(int x, int y)
        {
            int invertedY = Height - y - 1;
            int index = x + invertedY * Width;

            return index >= 0 && index < Squares.Count
                ? Squares[index]
                : NullSquare;
        }

        public void DestroyBoardAndPieces()
        {
            DestroyPieces();
            DestroyBoard();
            _boardInstance = null;
            WhitePieces.Clear();
            BlackPieces.Clear();
        }

        /// Get section relative to current piece color
        public Square GetSquareRel(PieceColor pieceColor, Square currentSquare, Vector2Int offset)
        {
            int x = -1;
            int y = -1;

            if (pieceColor == PieceColor.White)
            {
                x = currentSquare.X + offset.x;
                y = currentSquare.Y + offset.y;
            }
            else if (pieceColor == PieceColor.Black)
            {
                x = currentSquare.X - offset.x;
                y = currentSquare.Y - offset.y;
            }

            // if out of board bounds
            if (x < 0 || x >= Board.Width || y < 0 || y >= Board.Height)
            {
                return NullSquare;
            }

            return GetSquare(x, y);
        }

        /// Get section relative to absolute position (white side)
        public Square GetSquareAbs(Square currentSquare, Vector2Int offset)
        {
            return GetSquareRel(PieceColor.White, currentSquare, offset);
        }

        public void Build()
        {
            DestroyBoardAndPieces();
            FindAllSquares();
            HashSquares();
            LoadPiecesFromPreset();
            InstantiateBoard();
        }

        private void FindAllSquares()
        {
            // Get all squares components form parent tree
            foreach (Transform squareTransform in squaresParent)
            {
                Squares.Add(squareTransform.GetComponent<Square>());
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

        private void LoadPiecesFromPreset()
        {
            int x = 0;
            foreach (char ch in _boardPreset.PiecesPreset)
            {
                Square square = Squares[x];
                Piece piece;
                switch (ch)
                {
                    case '/':
                        continue;
                    case > '0' and <= '8':
                    {
                        int squaresToSkip = ch - '0';
                        x += squaresToSkip;
                        continue;
                    }
                    // White
                    case 'B':
                        piece = CreatePiece(PieceType.Bishop, PieceColor.White, square);
                        break;
                    case 'K':
                        piece = CreatePiece(PieceType.King, PieceColor.White, square);
                        CheckWhiteKingFirstMove(piece);
                        break;
                    case 'N':
                        piece = CreatePiece(PieceType.Knight, PieceColor.White, square);
                        break;
                    case 'P':
                        piece = CreatePiece(PieceType.Pawn, PieceColor.White, square);
                        CheckWhitePawnFirstMove(square, piece);
                        break;
                    case 'Q':
                        piece = CreatePiece(PieceType.Queen, PieceColor.White, square);
                        break;
                    case 'R':
                        piece = CreatePiece(PieceType.Rook, PieceColor.White, square);
                        CheckWhiteRookFirstMove(square, piece);
                        break;
                    // Black
                    case 'b':
                        piece = CreatePiece(PieceType.Bishop, PieceColor.Black, square);
                        break;
                    case 'k':
                        piece = CreatePiece(PieceType.King, PieceColor.Black, square);
                        CheckBlackKingFirstMove(piece);
                        break;
                    case 'n':
                        piece = CreatePiece(PieceType.Knight, PieceColor.Black, square);
                        break;
                    case 'p':
                        piece = CreatePiece(PieceType.Pawn, PieceColor.Black, square);
                        CheckBlackPawnFirstMove(square, piece);
                        break;
                    case 'q':
                        piece = CreatePiece(PieceType.Queen, PieceColor.Black, square);
                        break;
                    case 'r':
                        piece = CreatePiece(PieceType.Rook, PieceColor.Black, square);
                        CheckBlackRookFirstMove(square, piece);
                        break;
                    default:
                        Debug.LogError($"{ch} is not a valid character");
                        return;
                }

                piece.gameObject.SetActive(true);
                AddPiece(piece);
                x += 1;
            }
        }

        public EnPassantInfo GetEnPassantInfo()
        {
            string epSquareAddress = _commandBuffer.GetEpSquareAddress();
            if (epSquareAddress == "-")
            {
                epSquareAddress = _boardPreset.EnPassant;
            }
            epSquareAddress ??= "-";

            Square epSquare = GetSquare(epSquareAddress);

            if (epSquare == NullSquare)
            {
                return null;
            }

            // The (0, 1) to (0, -1) swapped because we find square for previous turn
            Square pawnToSquare = GetSquareRel(_turnColor, epSquare, new Vector2Int(0, -1));
            Piece pawn = pawnToSquare.GetPiece();

            return new EnPassantInfo(pawn, epSquare);
        }

        private static void CheckWhitePawnFirstMove(Square square, Piece piece)
        {
            if (square.Rank == "2")
            {
                piece.IsFirstMove = true;
            }
        }

        private static void CheckBlackPawnFirstMove(Square square, Piece piece)
        {
            if (square.Rank == "7")
            {
                piece.IsFirstMove = true;
            }
        }

        private void CheckBlackRookFirstMove(Square square, Piece piece)
        {
            if (_boardPreset.Castling.Contains("k") && square.Address == "h8"
                || _boardPreset.Castling.Contains("q") && square.Address == "a8")
            {
                piece.IsFirstMove = true;
            }
        }

        private void CheckWhiteRookFirstMove(Square square, Piece piece)
        {
            if (_boardPreset.Castling.Contains("K") && square.Address == "h1"
                || _boardPreset.Castling.Contains("Q") && square.Address == "a1")
            {
                piece.IsFirstMove = true;
            }
        }

        private void CheckBlackKingFirstMove(Piece piece)
        {
            if (_boardPreset.Castling.Contains("k")
                || _boardPreset.Castling.Contains("q"))
            {
                piece.IsFirstMove = true;
            }
        }

        private void CheckWhiteKingFirstMove(Piece piece)
        {
            if (_boardPreset.Castling.Contains("K")
                || _boardPreset.Castling.Contains("Q"))
            {
                piece.IsFirstMove = true;
            }
        }

        private void InstantiateBoard()
        {
            _boardInstance = Instantiate(_prefabs[12], transform);
        }

        public Piece GetOrCreatePiece(string piece, PieceColor pieceColor, string address)
        {
            Square square = GetSquare(address);
            if (square.HasPiece())
            {
                return square.GetPiece();
            }

            PieceType pieceType = GetPieceType(piece);

            return CreatePiece(pieceType, pieceColor, square);
        }

        // Get piece type from letter. Example: q, r, b, n
        public static PieceType GetPieceType(string piece)
        {
            PieceType pieceType = piece switch
            {
                "q" => PieceType.Queen,
                "r" => PieceType.Rook,
                "b" => PieceType.Bishop,
                "n" => PieceType.Knight,
                "p" => PieceType.Pawn,
                "k" => PieceType.King,
                _ => PieceType.None,
            };
            return pieceType;
        }

        public Piece CreatePiece(PieceType pieceType, PieceColor pieceColor, Square square)
        {
            GameObject piecePrefab = GetPrefabOfPiece(pieceType, pieceColor);
            Piece pieceInstance = InstantiatePiece(piecePrefab, square);

            return pieceInstance;
        }

        private GameObject GetPrefabOfPiece(PieceType pieceType, PieceColor pieceColor)
        {
            GameObject piecePrefab = pieceColor switch
            {
                PieceColor.White => pieceType switch
                {
                    // White
                    PieceType.Bishop => _prefabs[6],
                    PieceType.King => _prefabs[7],
                    PieceType.Knight => _prefabs[8],
                    PieceType.Pawn => _prefabs[9],
                    PieceType.Queen => _prefabs[10],
                    PieceType.Rook => _prefabs[11],
                    _ => null,
                },
                PieceColor.Black => pieceType switch
                {
                    // Black
                    PieceType.Bishop => _prefabs[0],
                    PieceType.King => _prefabs[1],
                    PieceType.Knight => _prefabs[2],
                    PieceType.Pawn => _prefabs[3],
                    PieceType.Queen => _prefabs[4],
                    PieceType.Rook => _prefabs[5],
                    _ => null,
                },
                _ => null,
            };
            return piecePrefab;
        }

        private Piece InstantiatePiece(GameObject piecePrefab, Square square)
        {
            if (piecePrefab == null)
            {
                return null;
            }

            GameObject pieceInstance = Instantiate(piecePrefab, square.transform.position,
                piecePrefab.transform.rotation, piecesParent);
            var piece = pieceInstance.GetComponent<Piece>();
            pieceInstance.SetActive(false);

            piece.Init(_game, this, square);

            return piece;
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

#if UNITY_EDITOR

        [Button(space: 10)]
        private void SetupSquaresInEditor()
        {
            FindAllSquaresInEditor();
            SetFilesAndRanks();
        }

        private void FindAllSquaresInEditor()
        {
            var undoObjects = new List<Object>();
            foreach (Transform squareTransform in squaresParent)
            {
                var square = squareTransform.GetComponent<Square>();
                Squares.Add(square);
                undoObjects.Add(square);
            }

            // Undo
            Undo.RecordObjects(undoObjects.ToArray(), "SetupSquaresInEditor");
            foreach (Object obj in undoObjects)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
            }
        }

        private void SetFilesAndRanks()
        {
            // Set files and ranks
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = x + y * Width;

                    Square square = Squares[index];
                    square.X = x;
                    square.Y = 7 - y;

                    square.File = $"{(char)(x + 'a')}";
                    square.Rank = $"{square.Y + 1}";
                    square.Address = $"{square.File}{square.Rank}";
                }
            }
        }

        // [Button(space: 10f)]
        private void ReformSquares()
        {
            var tr = squaresParent.Cast<Transform>().ToArray();
            int siblingInd = 0;
            int offset = 8;
            for (int currentInd = offset - 1; siblingInd < tr.Length;)
            {
                squaresParent.GetChild(currentInd).SetSiblingIndex(siblingInd);
                siblingInd += 1;

                if (currentInd == tr.Length - 1)
                {
                    offset -= 1;
                    currentInd = siblingInd + offset - 1;
                }
                else
                {
                    currentInd += offset;
                }
            }

            // SetFilesAndRanks();
        }

#endif
    }
}