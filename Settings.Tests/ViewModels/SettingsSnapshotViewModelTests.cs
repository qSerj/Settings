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
            Sections =
            {
                new SettingsSection
                {
                    Name = "Антенна",
                    Values = { new SettingValue { Key = "Gain", Value = "10" },
                               new SettingValue { Key = "Mode", Value = "Auto" } }
                },
                new SettingsSection
                {
                    Name = "Передатчик",
                    Values = { new SettingValue { Key = "Power", Value = "5W" } }
                }
            }
        };

        var vm = new SettingsSnapshotViewModel(snapshot);

        Assert.Equal("Антенна: 2; Передатчик: 1", vm.SectionsSummary);
    }
}
