using Chess3D.Runtime.Core.AssetManagement;
using Chess3D.Runtime.Core.ChessBoard.Info;
using Chess3D.Runtime.Core.ChessBoard.Pieces;
using Chess3D.Runtime.Core.Logic;
using Chess3D.Runtime.Core.Notation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Chess3D.Runtime.Core.ChessBoard
{
    [UnityEngine.Scripting.Preserve]
    public class PieceFactory
    {
        private readonly Assets _assets;
        private readonly IObjectResolver _resolver;

        public PieceFactory(Assets assets, IObjectResolver resolver)
        {
            _assets = assets;
            _resolver = resolver;
        }

        public Piece Create(PieceInfo pieceInfo)
        {
            GameObject piecePrefab = GetPrefabOfPiece(pieceInfo.PieceType, pieceInfo.PieceColor);
            var board = _resolver.Resolve<Board>();
            Square square = board.Squares[pieceInfo.SquareNum];
            Piece pieceInstance = InstantiatePiece(piecePrefab, square, board);
            pieceInstance.IsFirstMove = pieceInfo.IsFirstMove;

            return pieceInstance;
        }

        public Piece Create(PieceType pieceType, PieceColor pieceColor, Square square)
        {
            GameObject piecePrefab = GetPrefabOfPiece(pieceType, pieceColor);
            var board = _resolver.Resolve<Board>();
            Piece pieceInstance = InstantiatePiece(piecePrefab, square, board);

            return pieceInstance;
        }

        private GameObject GetPrefabOfPiece(PieceType pieceType, PieceColor pieceColor)
        {
            GameObject piecePrefab = pieceColor switch
            {
                PieceColor.Black => pieceType switch
                {
                    PieceType.Bishop => _assets.Prefabs.Pieceses[0],
                    PieceType.King => _assets.Prefabs.Pieceses[1],
                    PieceType.Knight => _assets.Prefabs.Pieceses[2],
                    PieceType.Pawn => _assets.Prefabs.Pieceses[3],
                    PieceType.Queen => _assets.Prefabs.Pieceses[4],
                    PieceType.Rook => _assets.Prefabs.Pieceses[5],
                    _ => null
                },
                PieceColor.White => pieceType switch
                {
                    PieceType.Bishop => _assets.Prefabs.Pieceses[6],
                    PieceType.King => _assets.Prefabs.Pieceses[7],
                    PieceType.Knight => _assets.Prefabs.Pieceses[8],
                    PieceType.Pawn => _assets.Prefabs.Pieceses[9],
                    PieceType.Queen => _assets.Prefabs.Pieceses[10],
                    PieceType.Rook => _assets.Prefabs.Pieceses[11],
                    _ => null
                },
                _ => null
            };

            return piecePrefab;
        }

        private Piece InstantiatePiece(GameObject piecePrefab, Square square, Board board)
        {
            if (!piecePrefab)
            {
                return null;
            }

            GameObject pieceInstance = _resolver.Instantiate(piecePrefab, square.transform.position,
                piecePrefab.transform.rotation, board.PiecesParent);
            var piece = pieceInstance.GetComponent<Piece>();
            pieceInstance.SetActive(false);

            var game = _resolver.Resolve<Game>();
            piece.Construct(game, board, square);

            return piece;
        }
    }
}