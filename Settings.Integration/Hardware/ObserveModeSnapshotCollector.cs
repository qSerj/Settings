using Settings.Core.Models;

namespace Settings.Integration.Hardware;

// Minimal collector for Observe mode.
public sealed class ObserveModeSnapshotCollector : IModeSnapshotCollector
{
    public string Mode => "Observe";

    public SettingsSnapshot Collect(RuntimeContext context)
    {
        return new SettingsSnapshot
        {
            Name = "HW Observe",
            Mode = Mode,
            UpdatedAt = DateTimeOffset.UtcNow,
            Radio = new RadioSettings
            {
                Rpu = new RpuSettings
                {
                    IsPresent = true,
                    IsRelevant = true
                },
                Detector = new DetectorSettings
                {
                    IsPresent = true,
                    IsRelevant = true
                }
            }
        };
    }
}
