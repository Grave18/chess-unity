using System;
using Chess3D.Runtime.Utilities;
using UniRx;

namespace Chess3D.Runtime.Core.Logic
{
    public interface IClock : ILoadUnit
    {
        public IObservable<TimeSpan> WhiteTime { get; }
        public IObservable<TimeSpan> BlackTime { get; }
        public IObservable<Unit> WhiteTenSeconds { get; }
        public IObservable<Unit> BlackTenSeconds { get; }
    }
}