using Board;
using Board.Pieces;
using Logic.CommandPattern;
using UnityEngine;

namespace Logic
{
    public class CommandManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private SeriesList seriesList;

        [SerializeField] private CommandBuffer _commandBuffer;

        public void MoveTo(Piece piece, Square square)
        {
            _commandBuffer.AddAndExecute(new MoveCommand(piece, square, gameManager, seriesList));
        }

        public void EatAt(Piece piece, Square square)
        {
            _commandBuffer.AddAndExecute(new EatCommand(piece, square, gameManager, seriesList));
        }

        public void Castling(King piece, Square kingSquare, Rook rook, Square rookSquare, TurnType turnType)
        {
            _commandBuffer.AddAndExecute(new CastlingCommand(piece, kingSquare, rook, rookSquare, gameManager, seriesList, turnType));
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