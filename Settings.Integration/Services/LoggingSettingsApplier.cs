using System.Threading;
using System.Threading.Tasks;
using Settings.Core.Interfaces;
using Settings.Core.Models;
using Settings.Core.Services;
using Serilog;

namespace Settings.Integration.Services;

public class LoggingSettingsApplier : ISettingsApplier
{
    private static readonly ILogger Logger = Log.ForContext<LoggingSettingsApplier>();
    private readonly SettingsApplier _inner;

    public LoggingSettingsApplier(SettingsApplier inner)
    {
        _inner = inner;
    }

    public async Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct)
    {
        Logger.Information(
            "Apply started. SnapshotId={SnapshotId}, Name={Name}, Mode={Mode}, UpdatedAt={UpdatedAt}",
            snapshot.Id,
            snapshot.Name,
            snapshot.Mode,
            snapshot.UpdatedAt);

        var result = await _inner.ApplyAsync(snapshot, reporter, ct);

        if (result.Success)
        {
            Logger.Information(
                "Apply finished successfully. SnapshotId={SnapshotId}, Mode={Mode}",
                snapshot.Id,
                snapshot.Mode);
        }
        else
        {
            Logger.Error(
                "Apply failed. SnapshotId={SnapshotId}, Mode={Mode}, Step={Step}, Error={Error}",
                snapshot.Id,
                snapshot.Mode,
                result.LastStep,
                result.Error);
        }

        return result;
    }
}
