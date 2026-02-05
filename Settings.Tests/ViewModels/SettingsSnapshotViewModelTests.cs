using Settings.Controls.ViewModels;
using Settings.Core.Models;
using Xunit;

namespace Settings.Tests.ViewModels;

public class SettingsSnapshotViewModelTests
{
    [Fact]
    public void SectionsSummary_ReturnsDashWhenEmpty()
    {
        var snapshot = new SettingsSnapshot();
        var vm = new SettingsSnapshotViewModel(snapshot);

        Assert.Equal("—", vm.SectionsSummary);
    }

    [Fact]
    public void SectionsSummary_BuildsReadableSummary()
    {
        var snapshot = new SettingsSnapshot
        {
            Radio = new RadioSettings
            {
                Antenna = new AntennaSettings
                {
                    Tuning =
                    {
                        new SettingValue { Key = "Azimuth", Value = "10" },
                        new SettingValue { Key = "Elevation", Value = "5" }
                    },
                    Reference =
                    {
                        new SettingValue { Key = "Type", Value = "Yagi" }
                    }
                },
                Rpu = new RpuSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "Gain", Value = "10" }
                    }
                }
            }
        };

        var vm = new SettingsSnapshotViewModel(snapshot);

        Assert.Equal("Antenna: 3; Rpu: 1", vm.SectionsSummary);
    }
}
