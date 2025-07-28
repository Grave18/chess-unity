using System;
using Initialization;
using UnityEngine;

namespace ChessGame.Logic
{
    public interface IClock
    {
        public TimeSpan WhiteTime {get;}
        public TimeSpan BlackTime {get;}

        public void Init(Game game, Vector2Int time);
    }
}