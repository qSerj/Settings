namespace Settings.Integration.Hardware;

public interface IChannelTargetContext
{
    object? CurrentTarget { get; }
    IDisposable Push(object target);
}
