using Board;
using Board.Pieces;
using UnityEngine;

namespace Logic.CommandPattern
{
    public class CommandManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private SeriesList seriesList;

        [SerializeField] private CommandBuffer _commandBuffer;

        public void MoveTo(Piece piece, Square square)
        {
            _commandBuffer.AddAndExecute(new MoveCommand(piece, square, gameManager.CurrentTurn, seriesList));
        }

        public void EatAt(Piece piece, Square square)
        {
            _commandBuffer.AddAndExecute(new EatCommand(piece, square, gameManager.CurrentTurn, seriesList));
        }

        [ContextMenu("Undo")]
        public void Undo()
        {
            _commandBuffer.Undo();
        }

        [ContextMenu("Redo")]
        public void Redo()
        {
            _commandBuffer.Redo();
        }
    }
}