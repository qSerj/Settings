using System;
using System.Threading;
using System.Threading.Tasks;
using Settings.Core.Interfaces;
using Settings.Core.Models;
using Serilog;

namespace Settings.Host.Services;

public class ObserveModeApplyStrategy : ISettingsApplyStrategy
{
    private static readonly ILogger Logger = Log.ForContext<ObserveModeApplyStrategy>();
    private readonly ISettingsSource _settingsSource;

    public string Mode => "Observe";

    public ObserveModeApplyStrategy(ISettingsSource settingsSource)
    {
        _settingsSource = settingsSource;
    }

    public async Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct)
    {
        const string stepInit = "Подготовка";

        var currentStep = stepInit;

        try
        {
            Logger.Information("Observe apply strategy started for snapshot {SnapshotId}", snapshot.Id);
            reporter.StepStarted(stepInit);
            await Task.Yield();
            reporter.StepSucceeded(stepInit);

            var radio = snapshot.Radio ?? new RadioSettings();

            var result = await ApplyNodeAsync(
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

            Logger.Information("Observe apply strategy finished successfully for snapshot {SnapshotId}", snapshot.Id);
            return ApplyResult.Ok();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Observe apply strategy failed at step {Step} for snapshot {SnapshotId}", currentStep, snapshot.Id);
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
            Logger.Information("Skipping node {Node} for snapshot {SnapshotId}: not present or not relevant", title, snapshot.Id);
            var skippedTitle = $"{title} (пропущено)";
            reporter.StepStarted(skippedTitle);
            await Task.Yield();
            reporter.StepSucceeded(skippedTitle);
            return null;
        }

        Logger.Information("Applying node {Node} for snapshot {SnapshotId}", title, snapshot.Id);
        reporter.StepStarted(title);
        try
        {
            await _settingsSource.ApplyAsync(BuildSnapshotForNode(snapshot, setNode));
            reporter.StepSucceeded(title);
            Logger.Information("Node {Node} applied successfully for snapshot {SnapshotId}", title, snapshot.Id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Node {Node} apply failed for snapshot {SnapshotId}", title, snapshot.Id);
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
