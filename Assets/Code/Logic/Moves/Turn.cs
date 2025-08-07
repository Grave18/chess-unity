namespace Logic.Moves
{
    public abstract class Turn
    {
        public abstract void Progress(float t, bool isUndo = false);

        public abstract void End();

        public abstract void EndUndo();

        public virtual void PlaySound()
        {

        }
    }
}