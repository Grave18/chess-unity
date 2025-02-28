﻿using ChessBoard;
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
        [SerializeField] private CommandBuffer commandBuffer;

        private void OnEnable()
        {
            game.OnStart += OnStart;
        }

        private void OnDisable()
        {
            game.OnStart -= OnStart;
        }

        public void MoveTo(Piece piece, Square square)
        {
            if (piece is Pawn && square.Rank is "8" or "1")
            {
                commandBuffer.AddAndExecute(new MoveAndPromoteCommand(piece, square, game, board, seriesList));
            }
            else
            {
                commandBuffer.AddAndExecute(new MoveCommand(piece, square, game, seriesList));
            }
        }

        /// <summary>
        /// Eat or eat and promote
        /// </summary>
        /// <param name="piece"> Piece being moved </param>
        /// <param name="moveToSquare"> Square where move piece is going </param>
        /// <param name="captureInfo"> Info with piece that is being eaten and notation type </param>
        public void EatAt(Piece piece, Square moveToSquare, CaptureInfo captureInfo)
        {
            // Promotion capture
            if (piece is Pawn && moveToSquare.Rank is "8" or "1")
            {
                commandBuffer.AddAndExecute(new EatAndPromoteCommand(piece, captureInfo.BeatenPiece, moveToSquare, game, board, seriesList));
            }
            // Capture
            else
            {
                commandBuffer.AddAndExecute(new EatCommand(piece, captureInfo.BeatenPiece, moveToSquare, game, seriesList, captureInfo.NotationTurnType));
            }
        }

        public void Castling(King piece, Square kingSquare, Rook rook, Square rookSquare, NotationTurnType notationTurnType)
        {
            commandBuffer.AddAndExecute(new CastlingCommand(piece, kingSquare, rook, rookSquare, game, seriesList, notationTurnType));
        }

        [ContextMenu("Undo")]
        public void Undo()
        {
            if(game.State != GameState.Idle)
            {
                return;
            }

            commandBuffer.Undo();
        }

        [ContextMenu("Redo")]
        public void Redo()
        {
            if(game.State != GameState.Idle)
            {
                return;
            }

            commandBuffer.Redo();
        }

        public string GetUciMoves()
        {
            return commandBuffer.GetUciMoves();
        }

        /// <summary>
        /// Get last moved piece from last buffer entry in command buffer
        /// </summary>
        /// <returns> Last moved piece </returns>
        public Piece GetLastMovedPiece()
        {
            return commandBuffer.GetLastMovedPiece();
        }

        private void OnStart()
        {
            commandBuffer.Clear();
            seriesList.Clear();
        }
    }
}