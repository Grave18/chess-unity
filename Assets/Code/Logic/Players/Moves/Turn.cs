using Logic.MovesBuffer;
using Logic.Players.GameStates;

namespace Logic.Players.Moves
{
    public abstract class Turn
    {
        protected ParsedUci ParsedUci { get; }
        protected MoveData MoveData { get; }

        protected Turn(ParsedUci parsedUci, MoveData moveData)
        {
            ParsedUci = parsedUci;
            MoveData = moveData;
        }

        public virtual void Progress(float t)
        {

        }

        public virtual void End()
        {

        }

        public virtual void EndUndo()
        {

        }
    }
}