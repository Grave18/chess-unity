using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Utility methods for working with tasks
    /// </summary>
    public static class TaskUtils
    {
        /// <summary>
        /// Waits until the given predicate is true, or the application is exiting
        /// </summary>
        /// <param name="predicate">The predicate to wait for</param>
        /// <param name="delay">The delay between checks, in milliseconds. Defaults to 25.</param>
        /// <returns>A task that completes when the predicate is true, or the cancellation token is cancelled.</returns>
        public static async Task WaitUntil(Func<bool> predicate, int delay = 25)
        {
            CancellationToken token = Application.exitCancellationToken;
            while (!predicate() && !token.IsCancellationRequested)
            {
                await Task.Delay(delay, token);
            }
        }
    }
}