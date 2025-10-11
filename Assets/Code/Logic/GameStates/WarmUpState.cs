namespace Logic.GameStates
{
    public class WarmUpState : GameState
    {
        public override string Name => "WarmUp";

        public WarmUpState(Game game) : base(game)
        {

        }

        public override void Enter()
        {
            Game.FireWarmup();
        }

        public override void Exit()
        {
            // Empty
        }

        public override void Move(string uci)
        {
            // Empty
        }

        public override void Undo()
        {
            // Empty
        }

        public override void Redo()
        {
            // Empty
        }

        public override void Play()
        {
            // Empty
        }

        public override void Pause()
        {
            // Empty
        }

        public override void Update()
        {
            // Empty
        }
    }
}