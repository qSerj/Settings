using System;
using System.Threading;
using System.Threading.Tasks;
using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Host.Services;

public class GlobalModeApplyStrategy : ISettingsApplyStrategy
{
    private readonly ISettingsSource _settingsSource;

    public string Mode => "Global";

    public GlobalModeApplyStrategy(ISettingsSource settingsSource)
    {
        _settingsSource = settingsSource;
    }

    public async Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct)
    {
        const string stepInit = "Подготовка";
        const string stepFinalize = "Завершение";

        var currentStep = stepInit;

        try
        {
            reporter.StepStarted(stepInit);
            await Task.Yield();
            reporter.StepSucceeded(stepInit);

            var radio = snapshot.Radio ?? new RadioSettings();

            var result = await ApplyNodeAsync(
                "Антенна",
                snapshot,
                radio.Antenna,
                r => r.Antenna = radio.Antenna,
                reporter);
            if (result != null) return result;

            result = await ApplyNodeAsync(
                "Приемник",
                snapshot,
                radio.Rpu,
                r => r.Rpu = radio.Rpu,
                reporter);
            if (result != null) return result;

            result = await ApplyNodeAsync(
                "Детектор",
                snapshot,
                radio.Detector,
                r => r.Detector = radio.Detector,
                reporter);
            if (result != null) return result;

            result = await ApplyNodeAsync(
                "Демодулятор",
                snapshot,
                radio.Demodulator,
                r => r.Demodulator = radio.Demodulator,
                reporter);
            if (result != null) return result;

            result = await ApplyNodeAsync(
                "Декодер",
                snapshot,
                radio.Decoder,
                r => r.Decoder = radio.Decoder,
                reporter);
            if (result != null) return result;

            currentStep = stepFinalize;
            reporter.StepStarted(stepFinalize);
            await Task.Yield();
            reporter.StepSucceeded(stepFinalize);

            return ApplyResult.Ok();
        }
        catch (Exception ex)
        {
            reporter.StepFailed(currentStep, ex.Message);
            return ApplyResult.Failed(ex.Message, currentStep);
        }
    }

    private static bool ShouldApply(SettingsBlock? block) =>
        block is { IsPresent: true, IsRelevant: true };

    private async Task<ApplyResult?> ApplyNodeAsync(
        string title,
        SettingsSnapshot snapshot,
        SettingsBlock? block,
        Action<RadioSettings> setNode,
        IApplyReporter reporter)
    {
        if (!ShouldApply(block))
        {
            var skippedTitle = $"{title} (пропущено)";
            reporter.StepStarted(skippedTitle);
            await Task.Yield();
            reporter.StepSucceeded(skippedTitle);
            return null;
        }

        reporter.StepStarted(title);
        try
        {
            await _settingsSource.ApplyAsync(BuildSnapshotForNode(snapshot, setNode));
            reporter.StepSucceeded(title);
            return null;
        }
        catch (Exception ex)
        {
            reporter.StepFailed(title, ex.Message);
            return ApplyResult.Failed(ex.Message, title);
        }
    }

    private static SettingsSnapshot BuildSnapshotForNode(
        SettingsSnapshot snapshot,
        Action<RadioSettings> setNode)
    {
        var radio = new RadioSettings();
        setNode(radio);

        return new SettingsSnapshot
        {
            Id = snapshot.Id,
            Name = snapshot.Name,
            Mode = snapshot.Mode,
            CreatedAt = snapshot.CreatedAt,
            UpdatedAt = snapshot.UpdatedAt,
            Radio = radio
        };
    }
}
