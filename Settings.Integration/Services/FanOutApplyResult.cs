namespace Settings.Integration.Services;

public sealed class FanOutApplyResult
{
    public IReadOnlyList<FanOutApplyChannelResult> Channels { get; init; } =
        Array.Empty<FanOutApplyChannelResult>();

    public bool Success => Channels.All(c => c.Success);
}

public sealed class FanOutApplyChannelResult
{
    public required string ChannelLabel { get; init; }
    public bool Success { get; init; }
    public string? Error { get; init; }
    public string? LastStep { get; init; }
}
