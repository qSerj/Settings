using Settings.Core.Interfaces;
using Settings.Core.Models;
using Settings.Integration.Hardware;

namespace Settings.Integration.Services;

public sealed class SettingsApplyFanOut : ISettingsApplyFanOut
{
    private readonly ISettingsApplier _applier;
    private readonly IChannelTargetResolver _targetResolver;
    private readonly IChannelTargetContext _targetContext;

    public SettingsApplyFanOut(
        ISettingsApplier applier,
        IChannelTargetResolver targetResolver,
        IChannelTargetContext targetContext)
    {
        _applier = applier;
        _targetResolver = targetResolver;
        _targetContext = targetContext;
    }

    public Task<ApplyResult> ApplyToActiveAsync(
        SettingsSnapshot snapshot,
        IApplyReporter? reporter,
        CancellationToken ct)
    {
        var active = _targetResolver.GetActiveChannel();
        using var scope = _targetContext.Push(active);
        return _applier.ApplyAsync(snapshot, reporter ?? NullApplyReporter.Instance, ct);
    }

    public async Task<FanOutApplyResult> ApplyToAllAsync(
        SettingsSnapshot snapshot,
        IApplyReporter? reporter,
        CancellationToken ct)
    {
        var channels = _targetResolver.GetAllChannels();
        var results = new List<FanOutApplyChannelResult>(channels.Count);
        foreach (var channel in channels)
        {
            ct.ThrowIfCancellationRequested();

            using var scope = _targetContext.Push(channel);
            var channelLabel = GetChannelLabel(channel);
            IApplyReporter prefixedReporter = reporter == null
                ? NullApplyReporter.Instance
                : new PrefixApplyReporter(reporter, $"[{channelLabel}] ");

            var result = await _applier.ApplyAsync(snapshot, prefixedReporter, ct);
            results.Add(new FanOutApplyChannelResult
            {
                ChannelLabel = channelLabel,
                Success = result.Success,
                Error = result.Error,
                LastStep = result.LastStep
            });
        }

        return new FanOutApplyResult
        {
            Channels = results
        };
    }

    private static string GetChannelLabel(object channel) =>
        channel.ToString() ?? "Channel";

    private sealed class PrefixApplyReporter : IApplyReporter
    {
        private readonly IApplyReporter _inner;
        private readonly string _prefix;

        public PrefixApplyReporter(IApplyReporter inner, string prefix)
        {
            _inner = inner;
            _prefix = prefix;
        }

        public void StepStarted(string title) => _inner.StepStarted(_prefix + title);

        public void StepSucceeded(string title) => _inner.StepSucceeded(_prefix + title);

        public void StepFailed(string title, string error) => _inner.StepFailed(_prefix + title, error);
    }

    private sealed class NullApplyReporter : IApplyReporter
    {
        public static readonly NullApplyReporter Instance = new();
        public void StepStarted(string title) { }
        public void StepSucceeded(string title) { }
        public void StepFailed(string title, string error) { }
    }
}
