using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Chess3D.Runtime.Utilities.Logging;
using Cysharp.Threading.Tasks;

namespace Chess3D.Runtime.Utilities
{
    public interface IDisposableLoadUnit : ILoadUnit, IDisposable { }

    public interface ILoadUnit
    {
        UniTask Load(CancellationToken cancellationToken = default);
    }

    public interface ILoadUnit<in T>
    {
        UniTask Load(T param, CancellationToken cancellationToken = default);
    }

    public interface IDisposableLoadUnit<in T> : ILoadUnit<T>, IDisposable { }

    public sealed class LoadingService
    {
        private readonly Stopwatch _watch =
#if !GRAVEBOX_PROD
            new();
#else
            null;
#endif

        public readonly List<IDisposable> Disposables = new();

        private void OnLoadingBegin(object unit)
        {
            _watch?.Restart();
            Log.Loading.D($"{unit.GetType().Name} loading is started");
        }

        private async UniTask OnLoadingFinish(object unit, bool isError, CancellationToken cancellationToken)
        {
            _watch?.Stop();
            Log.Loading.D($"{unit.GetType().Name} is {(isError ? "NOT " : "")}loaded with time {_watch?.ElapsedMilliseconds}ms");

            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            int mainThreadId = PlayerLoopHelper.MainThreadId;

            if (mainThreadId != currentThreadId) {
                _watch?.Restart();
                Log.Loading.D("THREADING",$"start switching from '{currentThreadId}' thread to main thread '{mainThreadId}'");
                await UniTask.SwitchToMainThread(cancellationToken);
                _watch?.Stop();
                Log.Loading.D("THREADING",$"switch finished with time {_watch?.ElapsedMilliseconds}");
            }
        }

        public async UniTask BeginLoading(ILoadUnit loadUnit, CancellationToken cancellationToken = default, bool skipExceptionThrow = false)
        {
            var isError = true;

            try {
                OnLoadingBegin(loadUnit);
                await loadUnit.Load(cancellationToken);
                isError = false;
            }
            catch (Exception e) {
                Log.Loading.E(e);

                if (!skipExceptionThrow)
                    throw;
            }
            finally {
                await OnLoadingFinish(loadUnit, isError, cancellationToken);
            }
        }

        public async UniTask BeginLoading(IDisposableLoadUnit unit, CancellationToken cancellationToken = default, bool skipExceptionThrow = false)
        {
            Disposables.Add(unit);
            await BeginLoading((ILoadUnit)unit, cancellationToken, skipExceptionThrow);
        }

        public async UniTask BeginLoading<T>(ILoadUnit<T> loadUnit, T param, CancellationToken cancellationToken = default, bool skipExceptionThrow = false)
        {
            var isError = true;

            try {
                OnLoadingBegin(loadUnit);
                await loadUnit.Load(param, cancellationToken);
                isError = false;
            }
            catch (OperationCanceledException) {
                throw;
            }
            catch (Exception e) {
                Log.Loading.E(e);

                if (!skipExceptionThrow)
                    throw;
            }
            finally {
                await OnLoadingFinish(loadUnit, isError, cancellationToken);
            }
        }

        public async UniTask BeginLoading<T>(IDisposableLoadUnit<T> unit, T param, CancellationToken cancellationToken = default, bool skipExceptionThrow = false)
        {
            Disposables.Add(unit);
            await BeginLoading((ILoadUnit<T>)unit, param, cancellationToken, skipExceptionThrow);
        }

        public async UniTask BeginLoading(CancellationToken cancellationToken = default, bool skipExceptionThrow = false, params ILoadUnit[] units)
        {
            foreach (ILoadUnit loadUnit in units)
                await BeginLoading(loadUnit, cancellationToken, skipExceptionThrow);
        }

        public async UniTask BeginLoadingParallel(string logName, CancellationToken cancellationToken = default, bool skipExceptionThrow = false, params ILoadUnit[] units)
        {
            var isError = true;

            try {
                OnLoadingBegin(logName);
                var t = UniTask.WhenAll(units.Select(e => e.Load(cancellationToken)));
                await t;
                isError = false;
            }
            catch (OperationCanceledException) {
                throw;
            }
            catch (Exception e) {
                Log.Loading.E(e);

                if (!skipExceptionThrow)
                    throw;
            }
            finally {
                await OnLoadingFinish(logName, isError, cancellationToken);
            }
        }
    }
}