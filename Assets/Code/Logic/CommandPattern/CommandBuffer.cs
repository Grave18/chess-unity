using UnityEngine;

namespace Logic.CommandPattern
{
    [System.Serializable]
    public class CommandBuffer
    {
        [SerializeField] private int cursor = -1;
        [SerializeField] private int length;
        [SerializeField] private int capacity = 2;

        private Command[] _commands = new Command[2];

        public int Length => length;
        public int Capacity => _commands.Length;

        public void AddAndExecute(Command command)
        {
            if (cursor == -1)
            {
                cursor += 1;
            }

            // Add command
            _commands[cursor] = command;

            // Execute
            _commands[cursor].Execute();
            cursor += 1;
            length = cursor;

            // Resize array
            ResizeArray();
        }

        // Redo
        public void Redo()
        {
            if(length == 0 || cursor == length)
            {
                return;
            }

            if (cursor == -1)
            {
                cursor += 1;
            }

            _commands[cursor].Execute();
            cursor += 1;
        }

        public void Undo()
        {
            if (length == 0 || cursor == -1)
            {
                return;
            }

            if (cursor == length)
            {
                cursor -= 1;
            }

            _commands[cursor].Undo();
            cursor -= 1;
        }

        private void ResizeArray()
        {
            if (cursor == Capacity)
            {
                var temp = new Command[Capacity * 2];

                for(int i = 0; i < Length; i++)
                {
                    temp[i] = _commands[i];
                }

                _commands = temp;

                capacity = _commands.Length;
            }
        }
    }
}
