using Board;
using Board.Pieces;
using UnityEngine;

namespace Logic.CommandPattern
{
    public class CommandHandler : MonoBehaviour
    {
        [SerializeField] private CommandBuffer _commandBuffer;

        public void MoveTo(Piece piece, Square square)
        {
            _commandBuffer.AddAndExecute(new MoveCommand(piece, square));
        }

        public void EatAt(Piece piece, Square square)
        {
            _commandBuffer.AddAndExecute(new EatCommand(piece, square));
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