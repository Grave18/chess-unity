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
            _commands[_cursor].Execute();
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
            _commands[_cursor].Execute();
        }

        public void Undo()
        {
            if (_cursor == -1)
            {
                return;
            }

            _commands[_cursor].Undo();
            _cursor -= 1;
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
