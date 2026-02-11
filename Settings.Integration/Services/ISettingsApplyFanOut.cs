using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Integration.Services;

public interface ISettingsApplyFanOut
{
    Task<ApplyResult> ApplyToActiveAsync(
        SettingsSnapshot snapshot,
        IApplyReporter? reporter,
        CancellationToken ct);

    Task<FanOutApplyResult> ApplyToAllAsync(
        SettingsSnapshot snapshot,
        IApplyReporter? reporter,
        CancellationToken ct);
}
