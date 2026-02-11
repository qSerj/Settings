using Settings.Core.Models;

namespace Settings.Integration.Hardware;

// Minimal collector for Global mode.
// Replace TODO sections with legacy object reads from RuntimeContext.Root.
public sealed class GlobalModeSnapshotCollector : IModeSnapshotCollector
{
    public string Mode => "Global";

    public SettingsSnapshot Collect(RuntimeContext context)
    {
        return new SettingsSnapshot
        {
            Name = "HW Global",
            Mode = Mode,
            UpdatedAt = DateTimeOffset.UtcNow,
            Radio = new RadioSettings
            {
                Antenna = new AntennaSettings
                {
                    IsPresent = true,
                    IsRelevant = true
                },
                Rpu = new RpuSettings
                {
                    IsPresent = true,
                    IsRelevant = true
                },
                Detector = new DetectorSettings
                {
                    IsPresent = true,
                    IsRelevant = true
                },
                Demodulator = new DemodulatorSettings
                {
                    IsPresent = true,
                    IsRelevant = true
                },
                Decoder = new DecoderSettings
                {
                    IsPresent = true,
                    IsRelevant = true
                }
            }
        };
    }
}
