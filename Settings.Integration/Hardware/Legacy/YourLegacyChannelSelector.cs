using System;

namespace Settings.Integration.Hardware.Legacy;

// Fill this adapter with access to your legacy channel registry/state.
public sealed class YourLegacyChannelSelector : ILegacyChannelSelector
{
    public object GetActiveChannel()
    {
        // TODO: return currently selected/active channel from your app.
        throw new InvalidOperationException("Implement active channel lookup in YourLegacyChannelSelector.");
    }

    public IReadOnlyList<object> GetAllChannels()
    {
        // TODO: return all currently available channels.
        return Array.Empty<object>();
    }
}
