﻿namespace Logic.Players.Moves
{
    public abstract class Turn
    {
        public abstract void Progress(float t);

        public abstract void End();

        public abstract void EndUndo();
    }
}