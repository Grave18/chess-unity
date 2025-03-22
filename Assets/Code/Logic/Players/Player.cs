namespace Logic.Players
{
    public abstract class Player
    {
        protected Game Game { get; private set; }

        protected Player(Game game)
        {
            Game = game;
        }

        public virtual void Update()
        {

        }

        public virtual void AllowMakeMove()
        {

        }

        public virtual void DisallowMakeMove()
        {

        }
    }
}