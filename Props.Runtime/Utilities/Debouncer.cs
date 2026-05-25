namespace Props.Runtime.Utilities;

/// <summary>
/// Delays execution of an action until a specified period of inactivity has elapsed, cancelling
/// any pending invocation when a new one arrives.
/// </summary>
public sealed class Debouncer(TimeSpan delay)
{
    private readonly Lock _syncRoot = new();
    private CancellationTokenSource _cts = new();

    /// <summary>
    /// Schedules <paramref name="action"/> to run after the debounce delay, restarting the timer
    /// if called again before the delay elapses.
    /// </summary>
    /// <param name="action">The action to invoke after the debounce period.</param>
    public async Task InvokeAsync(Func<CancellationToken, Task> action)
    {
        CancellationTokenSource currentCts;

        lock (_syncRoot)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            currentCts = _cts;
        }

        try
        {
            await Task.Delay(delay, currentCts.Token);
            await action(currentCts.Token);
        }
        catch (OperationCanceledException) when (currentCts.IsCancellationRequested)
        {
        }
    }
}
