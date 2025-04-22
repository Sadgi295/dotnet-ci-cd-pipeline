/// <summary>
/// Interface for counter operations that provides a consistent API for incrementing counters
/// </summary>
public interface ICounterService
{
    /// <summary>
    /// Atomically increments an internal counter and returns the new value
    /// </summary>
    /// <returns>The incremented counter value</returns>
    int IncrementAndGet();
}

/// <summary>
/// Thread-safe implementation of ICounterService that maintains an atomic counter
/// </summary>
public class CounterService : ICounterService
{
    // Internal counter, starts at 0
    private int _counter = 0;
    
    // Object used for synchronization to ensure thread safety
    private readonly object _lock = new object();

    /// <summary>
    /// Increments the counter by 1 in a thread-safe manner and returns the new value
    /// </summary>
    /// <returns>The updated counter value after increment</returns>
    /// <remarks>
    /// Uses a lock to ensure atomic operations across multiple threads.
    /// This prevents race conditions when multiple threads attempt to increment simultaneously.
    /// </remarks>
    public int IncrementAndGet()
    {
        lock (_lock)
        {
            return ++_counter;
        }
    }
}
