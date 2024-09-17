using System.Threading;

public interface IView
{
    /// <summary>
    /// Makes the <see cref="IView"/> visible
    /// </summary>
    void Show();
    /// <summary>
    /// Plays an animation that smoothly transitions from invisible to active
    /// </summary>
    UnityEngine.Awaitable PlayShowAnimation(CancellationToken cancellationToken = default);
    /// <summary>
    /// Plays an animation that smoothly transitions from inactive to invisible
    /// </summary>
    UnityEngine.Awaitable PlayHideAnimation(CancellationToken cancellationToken = default);

    /// <summary>
    /// Makes the <see cref="IView"/> invisible
    /// </summary>
    void Hide();
}