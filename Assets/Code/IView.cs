public interface IView
{
    UnityEngine.Awaitable Show(bool skipAnimation = false);
    UnityEngine.Awaitable Hide(bool skipAnimation = false);
}