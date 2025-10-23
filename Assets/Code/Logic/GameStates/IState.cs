namespace Logic.GameStates
{
    public interface IState
    {
        string Name { get; }

        void Move(string uci);
        void Undo();
        void Redo();
        void Play();
        void Pause();
    }
}