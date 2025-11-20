using System.Collections.Generic;
using System.Linq;
using Chess3D.Runtime.Core.AssetManagement;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Logic.MovesBuffer;
using Chess3D.Runtime.Core.Notation;
using Chess3D.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Chess3D.Runtime.Core.ChessBoard
{
    public sealed class Board : MonoBehaviour, ILoadUnit
    {
        public HashSet<Piece> WhitePieces { get; } = new();
        public HashSet<Piece> BlackPieces { get; } = new();
        public List<Square> Squares { get; } = new();

        // Inject part
        private PieceFactory pieceFactory;
        private Assets _assets;
        private UciBuffer _commandUciBuffer;
        private FenFromString _fenFromString;
        private SettingsService _settingsService;
        [SerializeField] private Transform piecesParent;
        [SerializeField] private Transform squaresParent;
        [SerializeField] private BeatenPieces beatenPieces;
        // TODO: remove this
        [SerializeField] private Square nullSquare;

        private Dictionary<string, Square> SquaresHash { get; } = new();
        private GameObject _boardInstance;

        private const int Width = 8;
        private const int Height = 8;

        public Square NullSquare => nullSquare;
        public Transform PiecesParent => piecesParent;

        [Inject]
        public void Construct(PieceFactory pieceFactory, Assets assets, UciBuffer commandUciBuffer, FenFromString fenFromString, SettingsService settingsService)
        {
            this.pieceFactory = pieceFactory;
            _assets = assets;
            _commandUciBuffer = commandUciBuffer;
            _fenFromString = fenFromString;
            _settingsService = settingsService;
        }

        public UniTask Load()
        {
            DestroyBoardAndPieces();
            beatenPieces.Clear();
            FindAllSquares();
            HashSquares();
            SpawnBoard();
            SpawnAllPieces();
            RotateBoard();

            return UniTask.CompletedTask;
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
            Square pawnToSquare = GetSquareRel(_fenFromString.TurnColor, epSquare, new Vector2Int(0, -1));
            Piece pawn = pawnToSquare.GetPiece();

            return new EnPassantInfo(pawn, epSquare);
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
            _boardInstance = Instantiate(_assets.Prefabs.Board, transform);
        }

        private void RotateBoard()
        {
            if (_settingsService.S.GameSettings.PlayerColor == PieceColor.White)
            {
                transform.localRotation = Quaternion.identity;
                beatenPieces.gameObject.transform.localScale = Vector3.one;
            }
            else if (_settingsService.S.GameSettings.PlayerColor == PieceColor.Black)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                beatenPieces.gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private void SpawnAllPieces()
        {
            foreach (PieceInfo pieceInfo in _fenFromString.PieceInfos)
            {
                Piece piece = pieceFactory.Create(pieceInfo);
                AddPiece(piece);
                piece.gameObject.SetActive(true);
            }
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