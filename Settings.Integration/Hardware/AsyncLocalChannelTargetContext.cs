using System.Threading;

namespace Settings.Integration.Hardware;

public sealed class AsyncLocalChannelTargetContext : IChannelTargetContext
{
    private readonly AsyncLocal<object?> _current = new();

    public object? CurrentTarget => _current.Value;

    public IDisposable Push(object target)
    {
        var previous = _current.Value;
        _current.Value = target;
        return new Scope(this, previous);
    }

    private sealed class Scope : IDisposable
    {
        private readonly AsyncLocalChannelTargetContext _owner;
        private readonly object? _previous;
        private bool _disposed;

        public Scope(AsyncLocalChannelTargetContext owner, object? previous)
        {
            _owner = owner;
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _owner._current.Value = _previous;
            _disposed = true;
        }
    }
}
