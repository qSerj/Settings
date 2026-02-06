using System.Collections.Generic;
using Settings.Core.Models;
using Settings.Core.Services;
using Xunit;

namespace Settings.Tests.Apply;

public class SettingsSourceBaseTests
{
    [Fact]
    public async Task ApplyAsync_AppliesAntennaFieldsAndSectionsInOrder()
    {
        var snapshot = new SettingsSnapshot
        {
            Radio = new RadioSettings
            {
                Antenna = new AntennaSettings
                {
                    AntennaId = "ANT-1",
                    Tuning =
                    {
                        new SettingValue { Key = "T1", Value = "1" },
                        new SettingValue { Key = "T2", Value = "2" }
                    },
                    Parameters =
                    {
                        new SettingValue { Key = "P1", Value = "A" }
                    },
                    Sections =
                    {
                        new SettingsSection
                        {
                            Name = "Main",
                            Values =
                            {
                                new SettingValue { Key = "S1", Value = "V1" }
                            },
                            Sections =
                            {
                                new SettingsSection
                                {
                                    Name = "Sub",
                                    Values =
                                    {
                                        new SettingValue { Key = "S2", Value = "V2" }
                                    }
                                }
                            }
                        }
                    },
                    Reference =
                    {
                        new SettingValue { Key = "R1", Value = "X" }
                    }
                }
            }
        };

        var source = new CaptureSettingsSource();

        await source.ApplyAsync(snapshot);

        Assert.Equal(
            new[]
            {
                "AntennaId:ANT-1",
                "Tuning:T1=1",
                "Tuning:T2=2",
                "Param:Antenna:P1=A",
                "Section:Antenna:Main:S1=V1",
                "Section:Antenna:Main/Sub:S2=V2"
            },
            source.Events);
        Assert.DoesNotContain(source.Events, e => e.StartsWith("Ref:"));
    }

    [Fact]
    public async Task ApplyAsync_SkipsNodesWhenNotPresentOrNotRelevant()
    {
        var snapshot = new SettingsSnapshot
        {
            Radio = new RadioSettings
            {
                Rpu = new RpuSettings
                {
                    IsPresent = false,
                    Parameters =
                    {
                        new SettingValue { Key = "P1", Value = "1" }
                    }
                }
            }
        };

        var source = new CaptureSettingsSource();

        await source.ApplyAsync(snapshot);

        Assert.Empty(source.Events);
    }

    [Fact]
    public async Task ApplyAsync_AppliesNodesInFixedOrder()
    {
        var snapshot = new SettingsSnapshot
        {
            Radio = new RadioSettings
            {
                Rpu = new RpuSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "R1", Value = "1" }
                    }
                },
                Antenna = new AntennaSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "A1", Value = "2" }
                    }
                }
            }
        };

        var source = new CaptureSettingsSource();

        await source.ApplyAsync(snapshot);

        Assert.Equal(
            new[]
            {
                "Param:Antenna:A1=2",
                "Param:Rpu:R1=1"
            },
            source.Events);
    }

    private sealed class CaptureSettingsSource : SettingsSourceBase
    {
        public List<string> Events { get; } = new();

        public override Task<SettingsSnapshot> GetCurrentAsync() =>
            Task.FromResult(new SettingsSnapshot());

        protected override Task ApplyAntennaIdAsync(string antennaId, CancellationToken ct)
        {
            Events.Add($"AntennaId:{antennaId}");
            return Task.CompletedTask;
        }

        protected override Task ApplyAntennaTuningAsync(SettingValue tuning, CancellationToken ct)
        {
            Events.Add($"Tuning:{tuning.Key}={tuning.Value}");
            return Task.CompletedTask;
        }

        protected override Task ApplyAntennaReferenceAsync(SettingValue reference, CancellationToken ct)
        {
            Events.Add($"Ref:{reference.Key}={reference.Value}");
            return Task.CompletedTask;
        }

        protected override Task ApplyParameterAsync(string nodeName, SettingValue parameter, CancellationToken ct)
        {
            Events.Add($"Param:{nodeName}:{parameter.Key}={parameter.Value}");
            return Task.CompletedTask;
        }

        protected override Task ApplySectionValueAsync(
            string nodeName,
            string sectionPath,
            SettingValue value,
            CancellationToken ct)
        {
            Events.Add($"Section:{nodeName}:{sectionPath}:{value.Key}={value.Value}");
            return Task.CompletedTask;
        }
    }
}
