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

        public void EatAt(Piece piece, Square square)
        {
            if (piece is Pawn && square.Rank is "8" or "1")
            {
                commandBuffer.AddAndExecute(new EatAndPromoteCommand(piece, square, gameManager, boardBuilder, seriesList));
            }
            else
            {
                commandBuffer.AddAndExecute(new EatCommand(piece, square, gameManager, seriesList));
            }
        }

        public void Castling(King piece, Square kingSquare, Rook rook, Square rookSquare, NotationTurnType notationTurnType)
        {
            commandBuffer.AddAndExecute(new CastlingCommand(piece, kingSquare, rook, rookSquare, gameManager, seriesList, notationTurnType));
        }

        [ContextMenu("Undo")]
        public void Undo()
        {
            commandBuffer.Undo();
        }

        [ContextMenu("Redo")]
        public void Redo()
        {
            commandBuffer.Redo();
        }

        private void Restart()
        {
            commandBuffer.Clear();
            seriesList.Clear();
        }
    }
}