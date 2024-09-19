using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class AsyncAnimator
{
    public delegate Awaitable AsyncAnimation(CancellationToken cancellationToken);

    private readonly Dictionary<string, Awaitable> _activeAnimations = new Dictionary<string, Awaitable>();
    private readonly Dictionary<string, CancellationTokenSource> _animationTokens = new Dictionary<string, CancellationTokenSource>();
    private readonly Dictionary<string, CancellationTokenSource> _animationGroupTokens = new Dictionary<string, CancellationTokenSource>();

    private readonly Dictionary<Awaitable, CancellationTokenSource> _cancellationTokens = new();


    public Awaitable Play(AsyncAnimation asyncAnimation)
    {
        return Play(null, asyncAnimation);
    }
    public async Awaitable Play(string cancellationKey, AsyncAnimation asyncAnimation)
    {
        CancellationTokenSource tokenSource = new();
        Awaitable animation = asyncAnimation(tokenSource.Token);
        if (cancellationKey != null)
        {
            // Any animation run without a key is valid, it just can't be cancelled via AsyncAnimator.Cancel()
            _animationTokens.Add(cancellationKey, tokenSource);
        }

        try { await animation; } catch (OperationCanceledException oce) { Debug.LogError($"Run() animation cancelled via Awaitable.Cancel()"); }

        tokenSource.Dispose();
    }

    public async IAsyncEnumerable<string> PlayMany(params AsyncAnimation[] animations)
    {
        await foreach (var item in PlayMany(null, animations))
        {
            yield return item;
        }
    }
    public async IAsyncEnumerable<string> PlayMany(string cancellationKey, params AsyncAnimation[] animations)
    {
        Dictionary<Awaitable, string> animationNames = new();
        Dictionary<Awaitable, CancellationTokenSource> animationTokenSources = new();

        if (cancellationKey != null)
        {
            Debug.Log($"--- Running group with key {cancellationKey} ---");
            CancellationTokenSource groupCTS = new CancellationTokenSource();
            _animationGroupTokens.Add(cancellationKey, groupCTS);
        }

        foreach (var item in animations)
        {
            // Kick off the concurrent processing of each animation
            CancellationTokenSource cts = new CancellationTokenSource();
            Awaitable animation = item.Invoke(cts.Token);

            // Store the values for later use
            animationTokenSources.Add(animation, cts);
            animationNames.Add(animation, item.Method.Name);
        }

        List<Awaitable> completedThisFrame = new();

        // While there are animations running, check each animation for completion
        while (animationTokenSources.Count > 0)
        {
            // If the batch is cancelled
            if (cancellationKey != null && _animationGroupTokens[cancellationKey].IsCancellationRequested)
            {
                // Tell each animation to cancel
                foreach (var anim in animationTokenSources)
                {
                    anim.Value.Cancel();

                    // Let them finish up
                    await anim.Key; // TODO this might cause some unexpected delayed behaviour if animations don't handle cancellation immediately...

                    // then dispose of the token source
                    anim.Value.Dispose();
                }
                break;
            }

            // Yield the name of each animation that has completed this frame for callers to clean up appropriately
            foreach (var tokenSource in animationTokenSources)
            {
                if (tokenSource.Key.IsCompleted)
                {
                    completedThisFrame.Add(tokenSource.Key);

                    yield return animationNames[tokenSource.Key];
                }
            }

            // Clean up completed animations
            foreach (var anim in completedThisFrame)
            {
                animationTokenSources.Remove(anim);
                animationNames.Remove(anim); // This is unnecessary, but better safe than confused later
            }
            completedThisFrame.Clear();

            // Give animations a frame to progress
            await Awaitable.NextFrameAsync();
        }

        _animationGroupTokens.Remove(cancellationKey);

        Debug.Log($"--- Animation group {cancellationKey} complete ---");
    }

    // TODO
    // In the event I want a handle on all tasks, there's benefit to creating the awaitable from the calling context and passing it in.
    // Pros: I can call myAwaitableVar.Cancel() from the outside
    // Cons: Still have to create cancellation token for it...
    // Either wait for all to finish, or have end up having idea which is finishing in which order, unless you do `if else (completed == myAwaitableVar)`, which... idk...
    /// <summary>
    /// <para>Warning: I have NO IDEA how Awaitable.Cancel() cancels it's tasks. If you cancel this batch, I can't promise things will be handled correctly because I don't have a handle on your Awaitable's cancellation tokens...</para>
    /// </summary>
    /// <param name="cancellationKey"></param>
    /// <param name="awaitables"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<Awaitable> RunConcurrently_DONTUSE(string cancellationKey, params Awaitable[] awaitables)
    {
        List<Awaitable> pendingAnimations = new List<Awaitable>(awaitables);

        if (cancellationKey != null)
        {
            Debug.Log($"Creating keyed CancellationTokenSource for {cancellationKey}");
            CancellationTokenSource groupCTS = new CancellationTokenSource();
            _animationTokens.Add(cancellationKey, groupCTS);
        }


        List<Awaitable> completedThisFrame = new();
        // While there are animations running, check each animation for completion
        while (pendingAnimations.Count > 0)
        {
            // If the batch is cancelled
            if (cancellationKey != null && _animationTokens[cancellationKey].IsCancellationRequested)
            {
                Debug.LogError($"Cancelling {cancellationKey} animation group relies on Awaitable.Cancel(), which leaves tasks in an incomplete state. Don't do this until I figure it the fuck out...");
                // Tell each animation to cancel
                foreach (var awaitable in pendingAnimations)
                {
                    awaitable.Cancel();

                    // Let them finish up
                    await awaitable; // TODO this might cause some unexpected delayed behaviour if animations don't handle cancellation immediately...
                }
                break;
            }

            // Yield the name of each animation that has completed this frame for callers to clean up appropriately
            foreach (var awaitable in pendingAnimations)
            {
                if (awaitable.IsCompleted)
                {
                    completedThisFrame.Add(awaitable);
                    yield return awaitable;
                }
            }

            // Clean up completed animations
            foreach (var anim in completedThisFrame)
            {
                pendingAnimations.Remove(anim);
            }
            completedThisFrame.Clear();

            // Give animations a frame to progress
            await Awaitable.NextFrameAsync();
        }
    }

    /// <summary>
    /// Safely* cancels any animation(s) registered with the given key
    /// <para>*Safely: any animation that doesn't handle token cancellation will continue to run and this function will be left waiting for it to finish</para>
    /// </summary>
    /// <param name="cancellationKey">The key used to when starting an animation (group)</param>
    public async Awaitable<bool> TryCancel(string cancellationKey)
    {
        if (_animationTokens.TryGetValue(cancellationKey, out CancellationTokenSource cts))
        {
            // Request animation stop
            cts.Cancel();
            Debug.Log($"Cancelling {cancellationKey} prematurely");

            if (_activeAnimations.TryGetValue(cancellationKey, out Awaitable animation))
            {
                // Wait for the task to finish up
                await animation;
                _activeAnimations.Remove(cancellationKey);
            }

            // Get rid of the token source
            try
            {
                cts.Dispose();
            }
            catch (ObjectDisposedException ode)
            {
                Debug.LogException(ode);
            }
            _animationTokens.Remove(cancellationKey);

            Debug.Log($"--- {cancellationKey} cancelled ---");
            return true;
        }
        else if (_animationGroupTokens.TryGetValue(cancellationKey, out CancellationTokenSource groupCts))
        {
            // Request animation stop
            cts.Cancel();
            Debug.Log($"Cancelling {cancellationKey} group prematurely");

            // Get rid of the token source
            try
            {
                cts.Dispose();
            }
            catch (ObjectDisposedException ode)
            {
                Debug.LogException(ode);
            }

            // _animationGroupTokens references are removed by their creators (PlayMany)
            Debug.Log($"--- {cancellationKey} group cancelled ---");
            return true;
        }
        else if (_activeAnimations.ContainsKey(cancellationKey))
        {
            Debug.Log($"Failed to find {cancellationKey} CancellationTokenSource");
            _animationTokens.Remove(cancellationKey);
            _activeAnimations.Remove(cancellationKey);
            return true;
        }

        return false;
    }
}