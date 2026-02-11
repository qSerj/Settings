using Settings.Core.Interfaces;
using Settings.Core.Models;
using Settings.Integration.Hardware;
using Settings.Integration.Services;
using Xunit;

namespace Settings.Tests.Apply;

public class SettingsApplyFanOutTests
{
    [Fact]
    public async Task ApplyToAllAsync_AppliesForEachChannel()
    {
        var context = new AsyncLocalChannelTargetContext();
        var applier = new RecordingApplier(context);
        var resolver = new StubResolver("A", "A", "B", "C");
        var fanOut = new SettingsApplyFanOut(applier, resolver, context);

        var result = await fanOut.ApplyToAllAsync(new SettingsSnapshot(), reporter: null, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(3, result.Channels.Count);
        Assert.Equal(new[] { "A", "B", "C" }, applier.AppliedTargets);
    }

    [Fact]
    public async Task ApplyToActiveAsync_AppliesOnlyActiveChannel()
    {
        var context = new AsyncLocalChannelTargetContext();
        var applier = new RecordingApplier(context);
        var resolver = new StubResolver("Current", "A", "B");
        var fanOut = new SettingsApplyFanOut(applier, resolver, context);

        var result = await fanOut.ApplyToActiveAsync(new SettingsSnapshot(), reporter: null, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Single(applier.AppliedTargets);
        Assert.Equal("Current", applier.AppliedTargets[0]);
    }

    private sealed class RecordingApplier : ISettingsApplier
    {
        public List<string> AppliedTargets { get; } = new();

        public Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct)
        {
            // Fan-out service writes current channel target to AsyncLocal context.
            var current = _context.CurrentTarget?.ToString() ?? "none";
            AppliedTargets.Add(current);
            return Task.FromResult(ApplyResult.Ok());
        }

        private readonly IChannelTargetContext _context;

        public RecordingApplier(IChannelTargetContext context)
        {
            _context = context;
        }
    }

    private sealed class StubResolver : IChannelTargetResolver
    {
        private readonly object _active;
        private readonly IReadOnlyList<object> _all;

        public StubResolver(object active, params object[] all)
        {
            _active = active;
            _all = all;
        }

        public object GetActiveChannel() => _active;

        public IReadOnlyList<object> GetAllChannels() => _all;
    }
}
