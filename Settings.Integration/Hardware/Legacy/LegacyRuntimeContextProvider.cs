using Settings.Integration.Hardware;

namespace Settings.Integration.Hardware.Legacy;

// Ready-to-fill runtime provider for legacy mode factories.
// It resolves active channel, determines current mode, and builds a mode-specific runtime object.
public sealed class LegacyRuntimeContextProvider : IRuntimeContextProvider
{
    private readonly ILegacyChannelSelector _channelSelector;
    private readonly ILegacyModeRuntimeFactory _runtimeFactory;

    public LegacyRuntimeContextProvider(
        ILegacyChannelSelector channelSelector,
        ILegacyModeRuntimeFactory runtimeFactory)
    {
        _channelSelector = channelSelector;
        _runtimeFactory = runtimeFactory;
    }

    public RuntimeContext GetCurrent()
    {
        var activeChannel = _channelSelector.GetActiveChannel();
        var mode = _runtimeFactory.GetMode(activeChannel);
        var modeRuntime = _runtimeFactory.BuildModeRuntime(mode, activeChannel);

        return new RuntimeContext
        {
            Mode = mode,
            Root = modeRuntime,
            ChannelTarget = activeChannel
        };
    }
}
