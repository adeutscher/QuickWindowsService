namespace QuickWindowsService.Services;

public interface IDelayWrapper
{
    Task DelaySecondsAsync(int valueSeconds, CancellationToken cancellationToken = default);
}

/// <summary>
///     Abstraction around Task.Delay.
///     Mostly for the sake of unit tests more than any program reason.
/// </summary>
internal class DelayWrapper : IDelayWrapper
{
    public Task DelaySecondsAsync(int valueSeconds, CancellationToken cancellationToken = default)
    {
        if (valueSeconds <= 0)
        {
            return Task.CompletedTask;
        }

        return Task.Delay(1000 * valueSeconds, cancellationToken);
    }
}