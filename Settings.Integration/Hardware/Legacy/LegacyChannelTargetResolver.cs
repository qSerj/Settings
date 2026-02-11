using Settings.Integration.Hardware;

namespace Settings.Integration.Hardware.Legacy;

// Bridges fan-out apply service to your legacy channel registry.
public sealed class LegacyChannelTargetResolver : IChannelTargetResolver
{
    private readonly ILegacyChannelSelector _channelSelector;

    public LegacyChannelTargetResolver(ILegacyChannelSelector channelSelector)
    {
        _channelSelector = channelSelector;
    }

    public object GetActiveChannel() => _channelSelector.GetActiveChannel();

    public IReadOnlyList<object> GetAllChannels() => _channelSelector.GetAllChannels();
}
