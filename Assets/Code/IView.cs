using System.Threading;

public interface IView
{
    /// <summary>
    /// Makes the <see cref="IView"/> visible
    /// </summary>
    void Show();
    /// <summary>
    /// Plays an animation that smoothly transitions from invisible to visible
    /// </summary>
    UnityEngine.Awaitable ShowAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Plays an animation that smoothly transitions from visible to invisible
    /// </summary>
    UnityEngine.Awaitable HideAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Makes the <see cref="IView"/> invisible
    /// </summary>
    void Hide();
}