﻿using Logic.MovesBuffer;

namespace Logic.Players.GameStates
{
    public class PauseState : GameState
    {
        public PauseState(Game game) : base(game)
        {
        }

        public override string Name => "Pause";
        public override void Enter()
        {
            Game.FirePause();
        }

        public override void Exit()
        {
            Game.FirePlay();
        }

        public override void Move(string uci)
        {
            // Can't move from Pause
        }

        public override void Undo()
        {
            if (Game.UciBuffer.CanUndo(out MoveData moveData))
            {
                Game.SetState(new UndoState(Game, moveData));
            }
        }

        public override void Redo()
        {
            if (Game.UciBuffer.CanRedo(out MoveData moveData))
            {
                Game.SetState(new RedoState(Game, moveData));
            }
        }

        public override void Play()
        {
            Game.SetState(new IdleState(Game));
        }

        public override void Pause()
        {
            // Already paused
        }

        public override void Update()
        {
            // Nothing to update
        }
    }
}