namespace Props.Runtime.Utilities;

public sealed class Debouncer(TimeSpan delay)
{
    private CancellationTokenSource _cts = new();

    public void Invoke(Action action)
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        var syncContext = SynchronizationContext.Current;
        Task.Delay(delay, token).ContinueWith(
            _ =>
            {
                if (syncContext is not null)
                    syncContext.Post(_ => action(), null);
                else
                    action();
            },
            token,
            TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
    }
}
