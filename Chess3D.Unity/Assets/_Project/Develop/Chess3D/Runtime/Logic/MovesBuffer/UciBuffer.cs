using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess3D.Runtime.Notation;
using UnityEngine;
using UnityEngine.Events;

namespace Chess3D.Runtime.Logic.MovesBuffer
{
    public class UciBuffer : MonoBehaviour
    {
        // State be cleared
        private LinkedListNode<MoveData> _head;
        private readonly LinkedList<MoveData> _buffer = new();
        private readonly Dictionary<string, int> _threefoldRepetition = new();

        public event UnityAction<MoveData> OnAdd;
        public event UnityAction<MoveData> OnUndo;
        public event UnityAction<MoveData> OnRedo;
        public event UnityAction<MoveData> OnDelete;

        private int _initialHalfMoveClock = 0;
        private int _initialFullMoveCounter = 1;
        private FenFromBoard _fenFromBoard;

        /// Returns half moves of the 50 moves rule
        public int HalfMoveClock => _head != null ? _head.Value.HalfMoveClock : _initialHalfMoveClock;
        public int FullMoveCounter => _head != null ? _head.Value.FullMoveCounter : _initialFullMoveCounter;

        /// Returns max repetition of the same position
        public int ThreefoldRepetitionCount => _threefoldRepetition.Values.Count > 0 ? _threefoldRepetition.Values.Max() : 0;

        public void Init(FenFromBoard fenFromBoard, FenFromString fenFromString)
        {
            _fenFromBoard = fenFromBoard;
            _initialHalfMoveClock = fenFromString.HalfMoveClock;
            _initialFullMoveCounter = fenFromString.FullMoveCounter;

            AddInitialPositionToThreefoldRule(fenFromString);
        }

        private void AddInitialPositionToThreefoldRule(FenFromString fenFromString)
        {
            string shortFen = fenFromString.GetShort();

            if (!_threefoldRepetition.TryAdd(shortFen, 1))
            {
                _threefoldRepetition[shortFen] += 1;
            }
        }

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

            RemoveRedundantMoves();
            AddThreefoldRule(moveData);

            OnAdd?.Invoke(newNode.Value);
        }

        private void RemoveRedundantMoves()
        {
            // Remove old redundant moves
            while (_buffer.Last != _head)
            {
                OnDelete?.Invoke(_buffer.Last.Value);
                _buffer.RemoveLast();
            }
        }

        private void AddThreefoldRule(MoveData moveData)
        {
            // Threefold repetition
            string shortFen = _fenFromBoard.GetShort();
            moveData.ThreefoldShortFen = shortFen;

            if (!_threefoldRepetition.TryAdd(shortFen, 1))
            {
                _threefoldRepetition[shortFen] += 1;
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
            OnUndo?.Invoke(_head?.Value);
            UndoThreefoldRule(_head);
            _head = _head?.Previous;
        }

        private void UndoThreefoldRule(LinkedListNode<MoveData> head)
        {
            string shortFen = head?.Value.ThreefoldShortFen ?? string.Empty;

            if (_threefoldRepetition.ContainsKey(shortFen))
            {
                _threefoldRepetition[shortFen] -= 1;
            }
        }

        public bool CanRedo(out MoveData moveData)
        {
            moveData = _head?.Next?.Value ?? _buffer.First?.Value;
            return _head is { Next: not null } || (_head is null && _buffer.First is not null);
        }

        public void Redo()
        {
            _head = _head?.Next ?? _buffer.First;
            OnRedo?.Invoke(_head?.Value);

            RedoThreefoldRule(_head);
        }

        private void RedoThreefoldRule(LinkedListNode<MoveData> head)
        {
            string shortFen = head?.Value.ThreefoldShortFen ?? string.Empty;

            if (_threefoldRepetition.ContainsKey(shortFen))
            {
                _threefoldRepetition[shortFen] += 1;
            }
        }

        public string GetEpSquareAddress()
        {
            string address = _head != null
                ? _head.Value.EpSquareAddress
                : string.Empty;

            return address;
        }

        public string GetMovesUci()
        {
            if (_buffer.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.Append("moves");
            foreach (MoveData entry in _buffer)
            {
                if (_head == null)
                {
                    break;
                }

                sb.Append($" {entry.Uci}");

                if (_head.Value == entry)
                {
                    break;
                }
            }

            return sb.ToString();
        }

        public string GetAllUciDebug()
        {
            var sb = new StringBuilder();
            foreach (MoveData entry in _buffer)
            {
                string color = entry == _head?.Value ? "white" : "grey";
                sb.Append($"<color={color}>{entry?.Uci}</color>\n");
            }

            return sb.ToString();
        }

        public void Clear()
        {
            _head = null;
            _buffer.Clear();
            ResetThreefoldRule();
        }

        private void ResetThreefoldRule()
        {
            string shortFen = _fenFromBoard.GetShort();

            _threefoldRepetition.Clear();
            _threefoldRepetition.Add(shortFen, 1);
        }
    }
}