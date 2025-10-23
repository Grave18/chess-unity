using PurrNet.StateMachine;

namespace Logic.GameStates
{
    public class WarmUpState : StateNode, IState
    {
        public string Name => "WarmUp";

        protected Game Game { get; private set; }

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

        public override void StateUpdate()
        {
            // Empty
        }

        public void Move(string uci)
        {
            // Empty
        }

        public void Undo()
        {
            // Empty
        }

        public void Redo()
        {
            // Empty
        }

        public void Play()
        {
            // Empty
        }

        public void Pause()
        {
            // Empty
        }
    }
}