using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Board;
using Board.Pieces;

namespace Logic
{
    public class AttackLine
    {
        public Piece Attacker { get; set; }
        public List<Square> Line  { get; set; }
        public bool IsCheck { get; set; }

        public bool Contains(Square moveSquare)
        {
            return Line.Contains(moveSquare);
        }
    }

    public class AttackLinesList : IEnumerable<AttackLine>
    {
        private List<AttackLine> _attackLines = new();

        public bool Contains(Square square, bool isCheck)
        {
            return isCheck
                ? _attackLines.Any(attackLine => attackLine.Contains(square) && attackLine.IsCheck)
                : _attackLines.Any(attackLine => attackLine.Contains(square));
        }

        public bool ContainsAttacker(Piece piece, bool isCheck)
        {
            return isCheck
                ? _attackLines.Any(attackLine => attackLine.Attacker == piece && attackLine.IsCheck)
                : _attackLines.Any(attackLine => attackLine.Attacker == piece);
        }

        public void Clear()
        {
            _attackLines.Clear();
        }

        public void Add(AttackLine attackLine)
        {
            _attackLines.Add(attackLine);
        }

        public object CountCheck()
        {
            return _attackLines.Count(attackLine => attackLine.IsCheck);
        }

        public IEnumerator<AttackLine> GetEnumerator()
        {
            return _attackLines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetAttackLine(Piece piece, out AttackLine attackLine)
        {
            attackLine = _attackLines.FirstOrDefault(attackLine => attackLine.Contains(piece.GetSquare()));
            return attackLine != null;
        }
    }
}