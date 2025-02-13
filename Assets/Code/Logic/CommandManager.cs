using Board;
using Board.Builder;
using Board.Pieces;
using Logic.CommandPattern;
using Logic.Notation;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic
{
    public class CommandManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private BoardBuilder boardBuilder;

        [SerializeField] private SeriesList seriesList;

        [FormerlySerializedAs("_commandBuffer")]
        [SerializeField] private CommandBuffer commandBuffer;

        private void OnEnable()
        {
            gameManager.OnRestart += Restart;
        }

        private void OnDisable()
        {
            gameManager.OnRestart -= Restart;
        }

        public void MoveTo(Piece piece, Square square)
        {
            if (piece is Pawn && square.Rank is "8" or "1")
            {
                commandBuffer.AddAndExecute(new MoveAndPromoteCommand(piece, square, gameManager, boardBuilder, seriesList));
            }
            else
            {
                commandBuffer.AddAndExecute(new MoveCommand(piece, square, gameManager, seriesList));
            }
        }

        /// <summary>
        /// Eat or eat and promote
        /// </summary>
        /// <param name="piece"> Piece being moved </param>
        /// <param name="beatenPiece"> Piece being eaten </param>
        /// <param name="moveToSquare"> Square where move piece is going </param>
        public void EatAt(Piece piece, Piece beatenPiece, Square moveToSquare)
        {
            if (piece is Pawn && moveToSquare.Rank is "8" or "1")
            {
                commandBuffer.AddAndExecute(new EatAndPromoteCommand(piece, beatenPiece, moveToSquare, gameManager, boardBuilder, seriesList));
            }
            else
            {
                commandBuffer.AddAndExecute(new EatCommand(piece, beatenPiece, moveToSquare, gameManager, seriesList));
            }
        }

        public void Castling(King piece, Square kingSquare, Rook rook, Square rookSquare, NotationTurnType notationTurnType)
        {
            commandBuffer.AddAndExecute(new CastlingCommand(piece, kingSquare, rook, rookSquare, gameManager, seriesList, notationTurnType));
        }

        [ContextMenu("Undo")]
        public void Undo()
        {
            if(gameManager.GameState != GameState.Idle)
            {
                return;
            }

            commandBuffer.Undo();
        }

        [ContextMenu("Redo")]
        public void Redo()
        {
            if(gameManager.GameState != GameState.Idle)
            {
                return;
            }

            commandBuffer.Redo();
        }

        /// <summary>
        /// Get last moved piece from last buffer entry in command buffer
        /// </summary>
        /// <returns> Last moved piece </returns>
        public Piece GetLastMovedPiece()
        {
            return commandBuffer.GetLastMovedPiece();
        }

        private void Restart()
        {
            commandBuffer.Clear();
            seriesList.Clear();
        }
    }
}