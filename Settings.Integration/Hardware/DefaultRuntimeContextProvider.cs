namespace Settings.Integration.Hardware;

// Placeholder provider for early integration.
// Replace with adapter that returns current mode and legacy runtime root object.
public sealed class DefaultRuntimeContextProvider : IRuntimeContextProvider
{
    public RuntimeContext GetCurrent() =>
        new()
        {
            Mode = "Global",
            Root = null,
            ChannelTarget = null
        };
}
