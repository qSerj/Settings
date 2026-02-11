using Settings.Core.Models;

namespace Settings.Integration.Hardware;

public interface IModeSnapshotCollector
{
    string Mode { get; }
    SettingsSnapshot Collect(RuntimeContext context);
}
