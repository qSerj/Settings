using System;

namespace Settings.Integration.Hardware;

// Placeholder resolver.
// Replace with integration that knows active/all channels in your application.
public sealed class DefaultChannelTargetResolver : IChannelTargetResolver
{
    public object GetActiveChannel()
    {
        throw new InvalidOperationException(
            "IChannelTargetResolver is not configured. Register your implementation.");
    }

    public IReadOnlyList<object> GetAllChannels() => Array.Empty<object>();
}
