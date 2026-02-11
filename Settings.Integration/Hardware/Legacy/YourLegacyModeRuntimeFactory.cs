using System;

namespace Settings.Integration.Hardware.Legacy;

// Fill this adapter with your "mode -> composed runtime object" factory logic.
public sealed class YourLegacyModeRuntimeFactory : ILegacyModeRuntimeFactory
{
    public string GetMode(object channel)
    {
        ArgumentNullException.ThrowIfNull(channel);

        // TODO: inspect channel and return current mode name used in snapshots
        // (for example: "Global", "Observe", ...).
        throw new InvalidOperationException("Implement mode resolution in YourLegacyModeRuntimeFactory.");
    }

    public object? BuildModeRuntime(string mode, object channel)
    {
        ArgumentNullException.ThrowIfNull(mode);
        ArgumentNullException.ThrowIfNull(channel);

        // TODO: create/resolve composed runtime object for current mode and channel.
        // This object will be passed into mode collectors via RuntimeContext.Root.
        return null;
    }
}
