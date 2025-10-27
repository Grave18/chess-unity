namespace Logic.GameStates
{
    public interface IGameState
    {
        string Name { get; }

        void Move(string uci);
        void Undo();
        void Redo();
        void Play();
        void Pause();
    }
}