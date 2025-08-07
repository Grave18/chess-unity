using Logic.Moves;
using Sound;

namespace Logic.GameStates
{
    public abstract class MovableState : GameState
    {
        protected Turn Turn;
        protected abstract float T { get; set; }
        protected abstract float SoundT { get; }

        protected MovableState(Game game) : base(game)
        {

        }

        protected void PlaySound(float delta)
        {
            // Not 0.5 sec because of duplicate sound
            float halfDelta = delta*0.525f;
            if (T >= SoundT - halfDelta && T <= SoundT + halfDelta)
            {
                Turn.PlaySound();
            }
        }

        protected void PlayCheckSound()
        {
            if (Game.IsCheck)
            {
                EffectsPlayer.Instance.PlayCheckSound();
            }
        }
    }
}