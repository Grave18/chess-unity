using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Chess3D.Runtime.Utilities.Common
{
    /// Utility methods for working with tasks
    public static class TaskUtils
    {
        /// Waits until the given predicate is true, or the application is exiting
        public static async Task WaitUntil(Func<bool> predicate, int delay = 25)
        {
            CancellationToken token = Application.exitCancellationToken;
            while (!predicate() && !token.IsCancellationRequested)
            {
                await Task.Delay(delay, token);
            }
        }

        /// Waits while the given predicate is true, or the application is exiting
        public static async Task WaitWhile(Func<bool> predicate, int delay = 25)
        {
            CancellationToken token = Application.exitCancellationToken;
            while (predicate() && !token.IsCancellationRequested)
            {
                await Task.Delay(delay, token);
            }
        }

        /// Continues a task on the Unity main thread. Throws if not called from the main thread
        public static void ContinueOnMainThread<T>(this Task<T> task, Action<Task<T>> continuation)
        {
            SynchronizationContext context = SynchronizationContext.Current;

            if (context == null || context.GetType().Name != "UnitySynchronizationContext")
            {
                throw new InvalidOperationException("No SynchronizationContext on current thread. Call from Unity main thread.");
            }

            task.ContinueWith(t =>
            {
                context.Post(_ => continuation(t), null);
            });
        }
    }
}