using Settings.Core.Interfaces;
using Settings.Core.Models;
using Settings.Core.Services;
using Xunit;

namespace Settings.Tests.Apply;

public class SettingsApplierTests
{
    [Fact]
    public async Task ApplyAsync_UsesMatchingStrategy()
    {
        var snapshot = new SettingsSnapshot { Mode = "Global" };
        var reporter = new CaptureReporter();
        var strategy = new TestStrategy("Global", ApplyResult.Ok());
        var applier = new SettingsApplier(new[] { strategy });

        var result = await applier.ApplyAsync(snapshot, reporter, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Empty(reporter.Failures);
        Assert.Equal(1, strategy.Calls);
    }

    [Fact]
    public async Task ApplyAsync_WhenNoStrategy_ReturnsFailed()
    {
        var snapshot = new SettingsSnapshot { Mode = "Unknown" };
        var reporter = new CaptureReporter();
        var applier = new SettingsApplier(Array.Empty<ISettingsApplyStrategy>());

        var result = await applier.ApplyAsync(snapshot, reporter, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("Unknown", result.Error);
        Assert.Single(reporter.Failures);
    }

    private sealed class TestStrategy : ISettingsApplyStrategy
    {
        public string Mode { get; }
        public int Calls { get; private set; }
        private readonly ApplyResult _result;

        public TestStrategy(string mode, ApplyResult result)
        {
            Mode = mode;
            _result = result;
        }

        public Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(_result);
        }
    }

    private sealed class CaptureReporter : IApplyReporter
    {
        public List<string> Failures { get; } = new();

        public void StepStarted(string title) { }

        public void StepSucceeded(string title) { }

        public void StepFailed(string title, string error)
        {
            Failures.Add($"{title}:{error}");
        }
    }
}
