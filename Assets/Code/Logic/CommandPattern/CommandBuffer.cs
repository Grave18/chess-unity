using System.Text;
using System.Threading.Tasks;

namespace Logic.CommandPattern
{
    public class CommandBuffer
    {
        private const int InitialCapacity = 32;

        private int _cursor = -1;
        private int _fullLength;
        private int _capacity = InitialCapacity;

        private Command[] _commands = new Command[InitialCapacity];

        /// Command what contains info about en passant
        public Command FirstCommand { private get; set; }

        /// Length of current commands in buffer under and to the left from cursor
        private int CurrentLength => _cursor + 1;

        public Command LastCommand => _cursor >= 0 && _cursor < _commands.Length ? _commands[_cursor] : FirstCommand;

        public async Task AddAndExecute(Command command)
        {
            Add(command);
            await Execute();
        }

        public void Add(Command command)
        {
            _cursor += 1;
            _fullLength = CurrentLength;

            ResizeArray();

            _commands[_cursor] = command;
        }

        public async Task Execute()
        {
            await LastCommand.Execute();
        }

        public async Task<Command> Redo()
        {
            if (!CanRedo())
            {
                return null;
            }

            _cursor += 1;
            await _commands[_cursor].Execute();
            return _commands[_cursor];
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
            return CurrentLength > 0 && LastCommand.IsUndoable;
        }

        public bool CanRedo()
        {
            return CurrentLength < _fullLength;
        }

        /// Get part of uci string, example: moves e2e4 e7e5
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

        public void Reset()
        {
            _cursor = -1;
            _fullLength = 0;
        }

        public void Clear()
        {
            FirstCommand = null;
            Reset();
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
