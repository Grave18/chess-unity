using System;
using Initialization;

namespace ChessGame.Logic
{
    public interface IClock
    {
        public TimeSpan WhiteTime {get;}
        public TimeSpan BlackTime {get;}

        public void Init(Game game, GameSettings gameSettings);
    }
}