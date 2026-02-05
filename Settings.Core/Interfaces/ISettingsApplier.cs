using Settings.Core.Models;

namespace Settings.Core.Interfaces;

public interface ISettingsApplier
{
    Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct);
}
