using System.Collections.Generic;
using System.Text;

namespace Logic.MovesBuffer
{
    public class Buffer
    {
        private readonly LinkedList<MoveData> _buffer = new();
        private LinkedListNode<MoveData> _head;

        public void Add(MoveData moveData)
        {
            var newNode = new LinkedListNode<MoveData>(moveData);
            if (_head == null)
            {
                _buffer.AddFirst(newNode);
            }
            else
            {
                _buffer.AddAfter(_head, newNode);
            }
            _head = newNode;

            while (_buffer.Last != _head)
            {
                _buffer.RemoveLast();
            }
        }

        public bool CanUndo(out MoveData moveData)
        {
            moveData = null;
            bool canUndo = _head != null;

            if (canUndo)
            {
                moveData = _head.Value;
            }

            return canUndo;
        }

        public void Undo()
        {
            _head = _head?.Previous;
        }

        public bool CanRedo(out string uci)
        {
            uci = _head?.Next?.Value.Uci ?? _buffer.First?.Value.Uci;
            return _head is { Next: not null } || (_head is null && _buffer.First is not null);
        }

        public void Redo()
        {
            _head = _head?.Next ?? _buffer.First;
        }

        public string GetEpSquareAddress()
        {
            string address = _head != null
                ? _head.Value.EpSquareAddress
                : "-";

            return address;
        }

        public string GetAllUciDebug()
        {
            var sb = new StringBuilder();
            foreach (MoveData entry in _buffer)
            {
                string color = entry == _head?.Value ? "white" : "grey";
                sb.Append($"<color={color}>{entry.Uci}</color>\n");
            }

            return sb.ToString();
        }

        public void Clear()
        {
            _buffer.Clear();
        }
    }
}