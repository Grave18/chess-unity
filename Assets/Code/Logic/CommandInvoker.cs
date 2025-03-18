using System.Threading.Tasks;
using AlgebraicNotation;
using ChessBoard;
using ChessBoard.Info;
using ChessBoard.Pieces;
using Logic.CommandPattern;
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

        /// Move piece to square or move and promote
        public async Task MoveTo(Piece piece, Square square, MoveInfo moveInfo)
        {
            game.StartTurn();

            // Promotion
            if (piece is Pawn && square.Rank is "8" or "1")
            {
                await _commandBuffer.AddAndExecute(new MoveAndPromoteCommand(piece, square, board));
            }
            // Normal move
            else
            {
                await _commandBuffer.AddAndExecute(new MoveCommand(piece, square, moveInfo));
            }

            game.EndTurn();
        }

        /// Capture or eat and promote
        public async Task EatAt(Piece piece, Square moveToSquare, CaptureInfo captureInfo)
        {
            game.StartTurn();

            // Promotion capture
            if (piece is Pawn && moveToSquare.Rank is "8" or "1")
            {
                await _commandBuffer.AddAndExecute(new EatAndPromoteCommand(piece, captureInfo.BeatenPiece,
                    moveToSquare, board));
            }
            // Capture
            else
            {
                await _commandBuffer.AddAndExecute(new EatCommand(piece, captureInfo.BeatenPiece, moveToSquare, captureInfo.NotationTurnType));
            }

            game.EndTurn();
        }

        public async Task Castling(King piece, Square kingSquare, Rook rook, Square rookSquare,
            NotationTurnType notationTurnType)
        {
            game.StartTurn();
            await _commandBuffer.AddAndExecute(new CastlingCommand(piece, kingSquare, rook, rookSquare, notationTurnType));
            game.EndTurn();
        }

        public async Task Undo()
        {
            if (IsStatePauseIdleThink() && _commandBuffer.CanUndo())
            {
                game.StartTurn(MoveType.Undo);

                await _commandBuffer.Undo();

                game.EndTurn();
            }
        }

        public async Task Redo()
        {
            if (IsStatePauseIdleThink() && _commandBuffer.CanRedo())
            {
                game.StartTurn(MoveType.Redo);

                _ = await _commandBuffer.Redo();

                game.EndTurn();
            }
        }

        private bool IsStatePauseIdleThink()
        {
            return game.State is GameState.Pause or GameState.Idle or GameState.Think;
        }

        /// Get part of uci string, example: moves e2e4 e7e5
        public string GetUciMoves()
        {
            return _commandBuffer.GetUciMoves();
        }

        /// Get last moved piece from last buffer entry in command buffer
        public Command GetLastCommand()
        {
            return _commandBuffer.LastCommand;
        }

        public int GetCommandsCount()
        {
            return _commandBuffer.CurrentLength;
        }

        /// Retrieves the En Passant information for the last command if applicable
        public EnPassantInfo GetEnPassantInfo()
        {
            Command command = _commandBuffer.LastCommand;
            if (command != null && command.EnPassantSquare != null)
            {
                return new EnPassantInfo(command.Piece, command.EnPassantSquare);
            }

            return null;
        }

        /// Set stub command what contains all info for en passant
        public void SetEnPassantInfo(EnPassantInfo enPassantInfo)
        {
            var firstCommand = new FirstCommand(enPassantInfo);
            _commandBuffer.FirstCommand = firstCommand;
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
            _commandBuffer.Reset();
            seriesList.Clear();
        }
    }
}