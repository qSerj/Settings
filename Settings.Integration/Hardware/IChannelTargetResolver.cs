namespace Settings.Integration.Hardware;

public interface IChannelTargetResolver
{
    object GetActiveChannel();
    IReadOnlyList<object> GetAllChannels();
}
