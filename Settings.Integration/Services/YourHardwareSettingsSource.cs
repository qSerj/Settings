using System;
using System.Threading.Tasks;
using Settings.Core.Models;
using Settings.Core.Services;

namespace Settings.Integration.Services;

// Minimal template for real hardware integration.
// Replace TODOs with actual adapter/API calls.
public class YourHardwareSettingsSource : SettingsSourceBase
{
    public override Task<SettingsSnapshot> GetCurrentAsync()
    {
        var snapshot = new SettingsSnapshot
        {
            Name = $"HW {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss}",
            Mode = "Global",
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
                }
            }
        };

        // TODO: read real hardware state and fill snapshot.Radio nodes.
        return Task.FromResult(snapshot);
    }

    protected override Task ApplyParameterAsync(string nodeName, SettingValue parameter, CancellationToken ct)
    {
        // TODO: route parameter write to your hardware API.
        return Task.CompletedTask;
    }

    protected override Task ApplySectionValueAsync(string nodeName, string sectionPath, SettingValue value, CancellationToken ct)
    {
        // TODO: route section value write to your hardware API.
        return Task.CompletedTask;
    }
}
