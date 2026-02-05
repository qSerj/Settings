using Settings.Core.Models;

namespace Settings.Core.Interfaces;

public interface ISettingsApplyStrategy
{
    string Mode { get; }
    Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct);
}
