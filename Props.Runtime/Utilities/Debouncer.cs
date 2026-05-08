namespace Props.Runtime.Utilities;

/// <summary>
/// Delays execution of an action until a specified period of inactivity has elapsed, cancelling
/// any pending invocation when a new one arrives.
/// </summary>
public sealed class Debouncer(TimeSpan delay)
{
    private CancellationTokenSource _cts = new();

    /// <summary>
    /// Schedules <paramref name="action"/> to run after the debounce delay, restarting the timer
    /// if called again before the delay elapses.
    /// </summary>
    /// <param name="action">The action to invoke after the debounce period.</param>
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
