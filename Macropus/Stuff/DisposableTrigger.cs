namespace Macropus.Stuff;

internal sealed class DisposableTrigger : IDisposable
{
    private readonly Action action;
    private bool disposed;

    public DisposableTrigger(Action action)
    {
        this.action = action;
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;

        action();
    }
}