using System.Text;
using System.Threading.Tasks;
using ChessBoard.Pieces;

namespace Logic.CommandPattern
{
    public class CommandBuffer
    {
        private const int InitialCapacity = 32;

        private int _cursor = -1;
        private int _fullLength;
        private int _capacity = InitialCapacity;

        private Command[] _commands = new Command[InitialCapacity];

        /// <summary>
        /// Length of current commands in buffer under and to the left from cursor
        /// </summary>
        private int CurrentLength => _cursor + 1;

        public async Task AddAndExecute(Command command)
        {
            _cursor += 1;

            // Rewrite fullLength only when add new move.
            // Rewrites moves right to cursor
            _fullLength = CurrentLength;
            ResizeArray();

            _commands[_cursor] = command;
            await _commands[_cursor].Execute();
        }

        public async Task Redo()
        {
            if (!CanRedo())
            {
                return;
            }

            _cursor += 1;
            await _commands[_cursor].Execute();
        }

        public async Task Undo()
        {
            if (!CanUndo())
            {
                return;
            }

            _cursor -= 1;
            await _commands[_cursor + 1].Undo();
        }

        public bool CanUndo()
        {
            return CurrentLength > 0;
        }

        public bool CanRedo()
        {
            return CurrentLength < _fullLength;
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

            if(CurrentLength == 0)
            {
                return string.Empty;
            }

            sb.Append("moves ");
            for (int i = 0; i < CurrentLength; i++)
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
            return CurrentLength == 0
                ? null
                : _commands[_cursor].GetPiece();
        }

        public void Clear()
        {
            _cursor = -1;
            _fullLength = 0;
        }

        private void ResizeArray()
        {
            if (CurrentLength >= _capacity)
            {
                var temp = new Command[_capacity * 2];

                for(int i = 0; i < _fullLength; i++)
                {
                    temp[i] = _commands[i];
                }

                _commands = temp;

                _capacity = _commands.Length;
            }
        }
    }
}
