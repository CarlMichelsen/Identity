namespace Test.Fakes;

/// <summary>
/// A fake TimeProvider for testing that allows manual control over the current time.
/// </summary>
public class FakeTimeProvider : TimeProvider
{
    private DateTimeOffset currentTime;
    private readonly object lockObject = new();

    public FakeTimeProvider(DateTimeOffset startTime)
    {
        currentTime = startTime;
    }

    public FakeTimeProvider() : this(DateTimeOffset.UtcNow)
    {
    }

    public override DateTimeOffset GetUtcNow()
    {
        lock (lockObject)
        {
            return currentTime;
        }
    }

    /// <summary>
    /// Advances the current time by the specified duration.
    /// </summary>
    public void Advance(TimeSpan duration)
    {
        lock (lockObject)
        {
            currentTime = currentTime.Add(duration);
        }
    }

    /// <summary>
    /// Sets the current time to a specific value.
    /// </summary>
    public void SetUtcNow(DateTimeOffset time)
    {
        lock (lockObject)
        {
            currentTime = time;
        }
    }
}
