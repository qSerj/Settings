using System;
using Settings.Core.Interfaces;
using Settings.Core.Models;
using Settings.Integration.Services;
using Xunit;

namespace Settings.Tests.Apply;

public class ApplyStrategiesTests
{
    [Fact]
    public async Task GlobalModeStrategy_ReportsStepsInOrder()
    {
        var snapshot = new SettingsSnapshot
        {
            Mode = "Global",
            Radio = new RadioSettings
            {
                Antenna = new AntennaSettings(),
                Rpu = new RpuSettings(),
                Detector = new DetectorSettings(),
                Demodulator = new DemodulatorSettings(),
                Decoder = new DecoderSettings()
            }
        };
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
                "Start:Антенна",
                "Ok:Антенна",
                "Start:Приемник",
                "Ok:Приемник",
                "Start:Детектор",
                "Ok:Детектор",
                "Start:Демодулятор",
                "Ok:Демодулятор",
                "Start:Декодер",
                "Ok:Декодер",
                "Start:Завершение",
                "Ok:Завершение"
            },
            reporter.Events);
    }

    [Fact]
    public async Task ObserveModeStrategy_ReportsStepsInOrder()
    {
        var snapshot = new SettingsSnapshot
        {
            Mode = "Observe",
            Radio = new RadioSettings
            {
                Rpu = new RpuSettings(),
                Detector = new DetectorSettings()
            }
        };
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
                "Start:Приемник",
                "Ok:Приемник",
                "Start:Детектор",
                "Ok:Детектор"
            },
            reporter.Events);
    }

    [Fact]
    public async Task Strategy_WhenApplyThrows_ReturnsFailed()
    {
        var snapshot = new SettingsSnapshot
        {
            Mode = "Global",
            Radio = new RadioSettings
            {
                Antenna = new AntennaSettings(),
                Rpu = new RpuSettings()
            }
        };
        var reporter = new CaptureReporter();
        var source = new TestSettingsSource { ThrowOnNode = "Приемник" };
        var strategy = new GlobalModeApplyStrategy(source);

        var result = await strategy.ApplyAsync(snapshot, reporter, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("boom", result.Error);
        Assert.Contains("Fail:Приемник", reporter.Events);
    }

    [Fact]
    public async Task GlobalModeStrategy_WhenNodeNotPresent_SkipsNode()
    {
        var snapshot = new SettingsSnapshot
        {
            Mode = "Global",
            Radio = new RadioSettings
            {
                Antenna = new AntennaSettings(),
                Rpu = new RpuSettings { IsPresent = false },
                Detector = new DetectorSettings()
            }
        };
        var reporter = new CaptureReporter();
        var source = new TestSettingsSource();
        var strategy = new GlobalModeApplyStrategy(source);

        var result = await strategy.ApplyAsync(snapshot, reporter, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Contains("Start:Приемник (пропущено)", reporter.Events);
        Assert.DoesNotContain(source.AppliedNodes, n => n == "Приемник");
    }

    private sealed class TestSettingsSource : ISettingsSource
    {
        public string? ThrowOnNode { get; set; }
        public List<string> AppliedNodes { get; } = new();

        public Task<SettingsSnapshot> GetCurrentAsync() =>
            Task.FromResult(new SettingsSnapshot());

        public Task ApplyAsync(SettingsSnapshot snapshot)
        {
            var node = DetectNode(snapshot.Radio);
            AppliedNodes.Add(node);

            if (!string.IsNullOrWhiteSpace(ThrowOnNode) &&
                string.Equals(node, ThrowOnNode, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("boom");
            }

            return Task.CompletedTask;
        }

        private static string DetectNode(RadioSettings radio) =>
            radio.Antenna != null ? "Антенна" :
            radio.Rpu != null ? "Приемник" :
            radio.Detector != null ? "Детектор" :
            radio.Demodulator != null ? "Демодулятор" :
            radio.Decoder != null ? "Декодер" :
            "Unknown";
    }

    private sealed class CaptureReporter : IApplyReporter
    {
        public List<string> Events { get; } = new();

        public void StepStarted(string title) => Events.Add($"Start:{title}");

        public void StepSucceeded(string title) => Events.Add($"Ok:{title}");

        public void StepFailed(string title, string error) => Events.Add($"Fail:{title}");
    }
}
