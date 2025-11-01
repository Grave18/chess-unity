using System;
using UnityEngine;

namespace Logic
{
    public interface IClock
    {
        public TimeSpan WhiteTime {get;}
        public TimeSpan BlackTime {get;}

        public void Init(Game game, Vector2Int time);
    }
}