using Logic.MovesBuffer;
using Logic.Players.GameStates;

namespace Logic.Players.Moves
{
    public class Castling : Turn
    {
        public Castling(ParsedUci parsedUci, MoveData moveData) : base(parsedUci, moveData)
        {
        }
    }
}