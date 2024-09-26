using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncAnimator
{
    public delegate Task AsyncAnimation(CancellationToken cancellationToken);

    private readonly Dictionary<string, CancellationTokenSource> _stringTokens = new Dictionary<string, CancellationTokenSource>();
    private readonly Dictionary<Task, CancellationTokenSource> _taskTokens = new Dictionary<Task, CancellationTokenSource>();

    /// <summary>
    /// Runs an uncancellable task
    /// </summary>
    /// <param name="asyncAnimation">An async func taking a <see cref="CancellationToken"/></param>
    public async Task Play(AsyncAnimation asyncAnimation)
    {
        CancellationTokenSource tokenSource = new();
        Task animation = asyncAnimation(tokenSource.Token);

        try { await animation; } catch (OperationCanceledException oce) { Debug.Log($"Unkeyed animation Canceled"); }

        tokenSource.Dispose();
    }
    public async Task Play(string cancelKey, AsyncAnimation asyncAnimation)
    {
        CancelIfActive(cancelKey);
        Debug.Log($"{cancelKey} - Playing");

        CancellationTokenSource tokenSource = new();
        Task animation = asyncAnimation(tokenSource.Token);
        _stringTokens.Add(cancelKey, tokenSource);

        try { await animation; } catch (OperationCanceledException oce) { Debug.Log($"{cancelKey} - Canceled"); }

        tokenSource.Dispose();
        _stringTokens.Remove(cancelKey);

        if (!animation.IsCanceled) Debug.Log($"{cancelKey} - Completed");
    }

    /// <summary>
    /// Cancels an animation if it is running
    /// </summary>
    /// <param name="cancelKey">The string passed to <see cref="AsyncAnimator.Play(string, AsyncAnimation)"/></param>
    public void CancelIfActive(string cancelKey)
    {
        if (_stringTokens.TryGetValue(cancelKey, out CancellationTokenSource cts))
        {
            Debug.Log($"{cancelKey} - Cancelling");
            cts.Cancel();
            cts.Dispose();
            _stringTokens.Remove(cancelKey);
        }
    }

    /// <summary>
    /// Allows the awaiting of multiple animations in parallel
    /// </summary>
    /// <param name="animations"></param>
    /// <returns>An iterator returning tasks in the order that they complete</returns>
    /// TODO It would be nice to have a means of cancelling the batch...
    public async IAsyncEnumerable<Task> WaitAll(params Task[] animations)
    {
        List<Task> tasks = animations.ToList();

        // While there are animations running, check each animation for completion
        while (tasks.Count > 0)
        {
            // Yield the task that has completed this frame for callers to clean up appropriately
            foreach (var task in tasks)
            {
                if (task.IsCompleted)
                {
                    if (_taskTokens.ContainsKey(task))
                    {
                        _taskTokens.Remove(task);
                    }

                    yield return task;
                }
            }

            // Give animations a frame to progress
            await Awaitable.NextFrameAsync();
        }
    }
}