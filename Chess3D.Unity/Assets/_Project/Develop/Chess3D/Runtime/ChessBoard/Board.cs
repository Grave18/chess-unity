using System.Collections.Generic;
using System.Linq;
using Chess3D.Runtime.ChessBoard.Info;
using Chess3D.Runtime.ChessBoard.Pieces;
using Chess3D.Runtime.Logic;
using Chess3D.Runtime.Logic.MovesBuffer;
using Chess3D.Runtime.Logic.Players;
using Chess3D.Runtime.Notation;
using UnityEngine;

namespace Chess3D.Runtime.ChessBoard
{
    public class Board : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Competitors competitors;
        [SerializeField] private BeatenPieces beatenPieces;
        [SerializeField] private Transform piecesParent;
        [SerializeField] private Transform squaresParent;
        [SerializeField] private Square nullSquare;

        public HashSet<Piece> WhitePieces { get; } = new();
        public HashSet<Piece> BlackPieces { get; } = new();
        public List<Square> Squares { get; } = new();
        private Dictionary<string, Square> SquaresHash { get; } = new();

        // Initialisation
        private Game _game;
        private UciBuffer _commandUciBuffer;
        private GameObject _boardPrefab;
        private IList<GameObject> _piecePrefabs;
        private FenFromString _fenFromString;
        private PieceColor _turnColor;

        private GameObject _boardInstance;

        public Square NullSquare => nullSquare;

        private const int Width = 8;
        private const int Height = 8;

        public void Init(Game game, UciBuffer commandUciBuffer, FenFromString fenFromString, GameObject boardPrefab,
            IList<GameObject> piecePrefabs, PieceColor turnColor)
        {
            _game = game;
            _commandUciBuffer = commandUciBuffer;
            _boardPrefab = boardPrefab;
            _piecePrefabs = piecePrefabs;
            _fenFromString = fenFromString;
            _turnColor = turnColor;
        }

        /// Add piece to the board. Add it to the list of the corresponding color
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
            if (squareAddress == null)
            {
                return NullSquare;
            }

            return SquaresHash.TryGetValue(squareAddress, out Square moveFromSquare)
                ? moveFromSquare
                : NullSquare;
        }

        /// Get section relative to absolute position (white side)
        public Square GetSquareAbs(Square currentSquare, Vector2Int offset)
        {
            return GetSquareRel(PieceColor.White, currentSquare, offset);
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

        // Get square by coordinates on the board. x: (0-7) left to right. y: (0-7) bottom to top
        private Square GetSquare(int x, int y)
        {
            int invertedY = Height - y - 1;
            int index = x + invertedY * Width;

            return index >= 0 && index < Squares.Count
                ? Squares[index]
                : NullSquare;
        }

        /// Get piece type from letter. Example: q, r, b, n
        public static PieceType GetPieceType(string pieceLetter)
        {
            PieceType pieceType = pieceLetter switch
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

        /// Get piece letter from piece. Example: q, r, b, n
        public static string GetPieceLetter(PieceType piece)
        {
            string pieceLetter = piece switch
            {
                PieceType.Queen => "q",
                PieceType.Rook => "r",
                PieceType.Bishop => "b",
                PieceType.Knight => "n",
                PieceType.Pawn => "p",
                PieceType.King => "k",
                _ => string.Empty,
            };

            return pieceLetter;
        }

        public EnPassantInfo GetEnPassantInfo()
        {
            string epSquareAddress = _commandUciBuffer.GetEpSquareAddress();

            if (epSquareAddress == string.Empty)
            {
                epSquareAddress = _fenFromString.EnPassantAddress;
            }

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

        public Piece GetSpawnedPiece(PieceType pieceType, PieceColor pieceColor, Square square)
        {
            GameObject piecePrefab = GetPrefabOfPiece(pieceType, pieceColor);
            Piece pieceInstance = InstantiatePiece(piecePrefab, square);

            return pieceInstance;
        }

        public void Build()
        {
            DestroyBoardAndPieces();
            beatenPieces.Clear();
            FindAllSquares();
            HashSquares();
            SpawnBoard();
            SpawnAllPieces();
        }

        private void DestroyBoardAndPieces()
        {
            DestroyPieces();
            DestroyBoard();
            _boardInstance = null;
            WhitePieces.Clear();
            BlackPieces.Clear();
        }

        private void FindAllSquares()
        {
            // Get all squares components form parent tree
            foreach (Transform squareTransform in squaresParent)
            {
                var square = squareTransform.GetComponent<Square>();
                square.SetPiece(null);
                Squares.Add(square);
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

        private void SpawnBoard()
        {
            _boardInstance = Instantiate(_boardPrefab, transform);
        }

        private void SpawnAllPieces()
        {
            foreach (PieceInfo pieceInfo in _fenFromString.PieceInfos)
            {
                Piece piece = GetSpawnedPiece(pieceInfo);
                AddPiece(piece);
                piece.gameObject.SetActive(true);
            }
        }

        private Piece GetSpawnedPiece(PieceInfo pieceInfo)
        {
            GameObject piecePrefab = GetPrefabOfPiece(pieceInfo.PieceType, pieceInfo.PieceColor);
            Square square = Squares[pieceInfo.SquareNum];
            Piece pieceInstance = InstantiatePiece(piecePrefab, square);
            pieceInstance.IsFirstMove = pieceInfo.IsFirstMove;

            return pieceInstance;
        }

        private GameObject GetPrefabOfPiece(PieceType pieceType, PieceColor pieceColor)
        {
            GameObject piecePrefab = pieceColor switch
            {
                PieceColor.Black => pieceType switch
                {
                    PieceType.Bishop => _piecePrefabs[0],
                    PieceType.King => _piecePrefabs[1],
                    PieceType.Knight => _piecePrefabs[2],
                    PieceType.Pawn => _piecePrefabs[3],
                    PieceType.Queen => _piecePrefabs[4],
                    PieceType.Rook => _piecePrefabs[5],
                    _ => null,
                },
                PieceColor.White => pieceType switch
                {
                    PieceType.Bishop => _piecePrefabs[6],
                    PieceType.King => _piecePrefabs[7],
                    PieceType.Knight => _piecePrefabs[8],
                    PieceType.Pawn => _piecePrefabs[9],
                    PieceType.Queen => _piecePrefabs[10],
                    PieceType.Rook => _piecePrefabs[11],
                    _ => null,
                },
                _ => null,
            };

            return piecePrefab;
        }

        private Piece InstantiatePiece(GameObject piecePrefab, Square square)
        {
            if (!piecePrefab)
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
            var pieces = piecesParent.Cast<Transform>();
            foreach (Transform piece in pieces)
            {
                Destroy(piece.gameObject);
            }
        }

        private void DestroyBoard()
        {
            if (!_boardInstance)
            {
                return;
            }

            Destroy(_boardInstance.gameObject);
        }
    }
}