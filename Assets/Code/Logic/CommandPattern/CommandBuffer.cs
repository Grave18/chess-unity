using System.Text;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    [System.Serializable]
    public class CommandBuffer
    {
        private const int InitialCapacity = 2;

        private int _cursor = -1;
        private int _length;
        private int _capacity = InitialCapacity;

        private Command[] _commands = new Command[InitialCapacity];

        public int Length => _length;
        public int Capacity => _capacity;

        public void AddAndExecute(Command command)
        {
            _cursor += 1;

            _commands[_cursor] = command;
            _commands[_cursor].ExecuteAsync();
            _length = _cursor + 1;

            ResizeArray();
        }

        public void Redo()
        {
            if (_cursor + 1 == _length)
            {
                return;
            }

            _cursor += 1;
            _commands[_cursor].ExecuteAsync();
        }

        public void Undo()
        {
            if (_cursor == -1)
            {
                return;
            }

            _commands[_cursor].UndoAsync();
            _cursor -= 1;
        }

        /// <summary>
        /// Get part of uci string
        /// </summary>
        /// <example>
        /// "moves e2e4 e7e5"
        /// </example>
        /// <returns> Uci string </returns>
        public string GetUciMoves()
        {
            var sb = new StringBuilder();

            if(Length == 0)
            {
                return string.Empty;
            }

            sb.Append("moves ");
            for (int i = 0; i < Length; i++)
            {
                sb.Append(_commands[i].UciMove);
                sb.Append(' ');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the last moved piece from last buffer entry
        /// </summary>
        /// <returns> Last moved piece </returns>
        public Piece GetLastMovedPiece()
        {
            return _cursor == -1
                ? null
                : _commands[_cursor].GetPiece();
        }

        public void Clear()
        {
            _cursor = -1;
            _length = 0;
        }

        private void ResizeArray()
        {
            if (_cursor + 1 == _capacity)
            {
                var temp = new Command[_capacity * 2];

                for(int i = 0; i < Length; i++)
                {
                    temp[i] = _commands[i];
                }

                _commands = temp;

                _capacity = _commands.Length;
            }
        }
    }
}
