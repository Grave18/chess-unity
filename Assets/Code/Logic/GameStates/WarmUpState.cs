namespace Logic.GameStates
{
    public class WarmUpState : GameState
    {
        protected Game Game { get; private set; }
        public override string Name => "WarmUp";

        public WarmUpState(Game game)
        {
            Game = game;
        }

        public override void Enter()
        {
            Game.ResetGameState();
            Game.PreformCalculations();
            Game.FireWarmup();
        }

        public override void Exit()
        {
            Game.FireStart();
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