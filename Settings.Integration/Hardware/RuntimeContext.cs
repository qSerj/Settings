namespace Settings.Integration.Hardware;

public sealed class RuntimeContext
{
    public required string Mode { get; init; }
    public object? Root { get; init; }
    public object? ChannelTarget { get; init; }
}
