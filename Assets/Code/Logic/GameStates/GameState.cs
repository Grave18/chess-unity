namespace Logic.GameStates
{
    public abstract class GameState
    {
        public abstract string Name { get; }
        protected Game Game { get; private set; }

        protected GameState(Game game)
        {
            Game = game;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Move(string uci);
        public abstract void Undo();
        public abstract void Redo();
        public abstract void Play();
        public abstract void Pause();
        public abstract void Update();
    }
}