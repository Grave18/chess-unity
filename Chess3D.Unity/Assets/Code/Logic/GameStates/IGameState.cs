namespace Logic.GameStates
{
    public interface IGameState
    {
        string Name { get; }

        void Enter();
        void Exit();
        void StateUpdate();

        void Move(string uci);
        void Undo();
        void Redo();
        void Play();
        void Pause();
    }

    public interface IGameState<in T>: IGameState
    {
        void Enter(T data);
    }
}