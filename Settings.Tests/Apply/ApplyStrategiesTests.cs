using Settings.Core.Interfaces;
using Settings.Core.Models;
using Settings.Host.Services;
using Xunit;

namespace Settings.Tests.Apply;

public class ApplyStrategiesTests
{
    [Fact]
    public async Task GlobalModeStrategy_ReportsStepsInOrder()
    {
        var snapshot = new SettingsSnapshot { Mode = "Global" };
        var reporter = new CaptureReporter();
        var source = new TestSettingsSource();
        var strategy = new GlobalModeApplyStrategy(source);

        var result = await strategy.ApplyAsync(snapshot, reporter, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(
            new[]
            {
                "Start:Подготовка",
                "Ok:Подготовка",
                "Start:Применение настроек",
                "Ok:Применение настроек",
                "Start:Завершение",
                "Ok:Завершение"
            },
            reporter.Events);
    }

    [Fact]
    public async Task ObserveModeStrategy_ReportsStepsInOrder()
    {
        var snapshot = new SettingsSnapshot { Mode = "Observe" };
        var reporter = new CaptureReporter();
        var source = new TestSettingsSource();
        var strategy = new ObserveModeApplyStrategy(source);

        var result = await strategy.ApplyAsync(snapshot, reporter, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(
            new[]
            {
                "Start:Подготовка",
                "Ok:Подготовка",
                "Start:Применение мониторинга",
                "Ok:Применение мониторинга"
            },
            reporter.Events);
    }

    [Fact]
    public async Task Strategy_WhenApplyThrows_ReturnsFailed()
    {
        var snapshot = new SettingsSnapshot { Mode = "Global" };
        var reporter = new CaptureReporter();
        var source = new TestSettingsSource { ThrowOnApply = true };
        var strategy = new GlobalModeApplyStrategy(source);

        var result = await strategy.ApplyAsync(snapshot, reporter, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("boom", result.Error);
        Assert.Contains("Fail:Применение настроек", reporter.Events);
    }

    private sealed class TestSettingsSource : ISettingsSource
    {
        public bool ThrowOnApply { get; set; }

        public Task<SettingsSnapshot> GetCurrentAsync() =>
            Task.FromResult(new SettingsSnapshot());

        public Task ApplyAsync(SettingsSnapshot snapshot)
        {
            if (ThrowOnApply)
            {
                throw new InvalidOperationException("boom");
            }

            return Task.CompletedTask;
        }
    }

    private sealed class CaptureReporter : IApplyReporter
    {
        public List<string> Events { get; } = new();

        public void StepStarted(string title) => Events.Add($"Start:{title}");

        public void StepSucceeded(string title) => Events.Add($"Ok:{title}");

        public void StepFailed(string title, string error) => Events.Add($"Fail:{title}");
    }
}
