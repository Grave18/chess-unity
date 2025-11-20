using System;
using Chess3D.Runtime.Utilities;

namespace Chess3D.Runtime.Core.Logic
{
    public interface IClock : ILoadUnit
    {
        public TimeSpan WhiteTime {get;}
        public TimeSpan BlackTime {get;}
    }
}