using Logic.MovesBuffer;
using PurrNet.StateMachine;
using UnityEngine;

namespace Logic.GameStates
{
    public class EndGameState : StateNode, IState
    {
        [Header("References")]
        [SerializeField] private Game game;

        public string Name => "End Game";

        public override void Enter()
        {
            game.FireEnd();
        }

        public override void Exit()
        {
            // Empty
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
            if (game.UciBuffer.CanUndo(out MoveData moveData))
            {
                // TODO: Game.GameStateMachine.SetState(new UndoState(Game, moveData), isSetPreviousState:false);
            }
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