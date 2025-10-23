using PurrNet.StateMachine;
using UnityEngine;

namespace Logic.GameStates
{
    public class WarmUpState : StateNode, IState
    {
        [Header("References")]
        [SerializeField] private Game game;

        public string Name => "WarmUp";

        public override void Enter()
        {
            game.ResetGameState();
            game.PreformCalculations();
            game.FireWarmup();
        }

        public override void Exit()
        {
            game.FireStart();
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