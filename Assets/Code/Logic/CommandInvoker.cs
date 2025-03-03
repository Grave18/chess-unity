using System.Threading.Tasks;
using ChessBoard;
using ChessBoard.Pieces;
using Logic.CommandPattern;
using Logic.Notation;
using UnityEngine;

namespace Logic
{
    public class CommandInvoker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Game game;
        [SerializeField] private Board board;
        [SerializeField] private SeriesList seriesList;

        private readonly CommandBuffer _commandBuffer = new();

        /// <summary>
        /// Move piece to square
        /// </summary>
        /// <param name="piece"> Piece being moved </param>
        /// <param name="square"> Square where move piece is going </param>
        public async Task MoveTo(Piece piece, Square square)
        {
            game.StartTurn();

            // Promotion
            if (piece is Pawn && square.Rank is "8" or "1")
            {
                await _commandBuffer.AddAndExecute(new MoveAndPromoteCommand(piece, square, game, board, seriesList));
            }
            // Normal move
            else
            {
                await _commandBuffer.AddAndExecute(new MoveCommand(piece, square, game, seriesList));
            }

            game.EndTurn();
        }

        /// <summary>
        /// Eat or eat and promote
        /// </summary>
        /// <param name="piece"> Piece being moved </param>
        /// <param name="moveToSquare"> Square where move piece is going </param>
        /// <param name="captureInfo"> Info with piece that is being eaten and notation type </param>
        public async Task EatAt(Piece piece, Square moveToSquare, CaptureInfo captureInfo)
        {
            game.StartTurn();

            // Promotion capture
            if (piece is Pawn && moveToSquare.Rank is "8" or "1")
            {
                await _commandBuffer.AddAndExecute(new EatAndPromoteCommand(piece, captureInfo.BeatenPiece, moveToSquare, game, board, seriesList));
            }
            // Capture
            else
            {
                await _commandBuffer.AddAndExecute(new EatCommand(piece, captureInfo.BeatenPiece, moveToSquare, game, seriesList, captureInfo.NotationTurnType));
            }

            game.EndTurn();
        }


        /// <summary>
        /// Execute castling
        /// </summary>
        /// <param name="piece"> Piece being moved </param>
        /// <param name="kingSquare"> Square where king is going </param>
        /// <param name="rook"> Rook being moved </param>
        /// <param name="rookSquare"> Square where rook is going </param>
        /// <param name="notationTurnType"> Castling left or right notation</param>
        public async Task Castling(King piece, Square kingSquare, Rook rook, Square rookSquare, NotationTurnType notationTurnType)
        {
            game.StartTurn();
            await _commandBuffer.AddAndExecute(new CastlingCommand(piece, kingSquare, rook, rookSquare, game, seriesList, notationTurnType));
            game.EndTurn();
        }

        public async Task Undo()
        {
            if(IsRightState() && _commandBuffer.CanUndo())
            {
                game.StartTurn();
                await _commandBuffer.Undo();
                game.EndTurn(isPause: true);
            }
        }

        public async Task Redo()
        {
            if(IsRightState() && _commandBuffer.CanRedo())
            {
                game.StartTurn();
                await _commandBuffer.Redo();
                game.EndTurn(isPause: true);
            }
        }

        public string GetUciMoves()
        {
            return _commandBuffer.GetUciMoves();
        }

        /// <summary>
        /// Get last moved piece from last buffer entry in command buffer
        /// </summary>
        /// <returns> Last moved piece </returns>
        public Piece GetLastMovedPiece()
        {
            return _commandBuffer.GetLastMovedPiece();
        }

        private bool IsRightState()
        {
            return game.State is GameState.Pause or GameState.Idle;
        }

        private void OnEnable()
        {
            game.OnStart += OnStart;
        }

        private void OnDisable()
        {
            game.OnStart -= OnStart;
        }

        private void OnStart()
        {
            _commandBuffer.Clear();
            seriesList.Clear();
        }
    }
}