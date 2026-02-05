using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Host.Services;

public class MockSettingsSource : ISettingsSource
{
    private int _counter;

    public Task<SettingsSnapshot> GetCurrentAsync()
    {
        _counter++;
        var snapshot = new SettingsSnapshot
        {
            Name = $"Снимок {_counter}",
            Mode = _counter % 2 == 0 ? "Observe" : "Global",
            UpdatedAt = DateTimeOffset.UtcNow,
            Radio = new RadioSettings
            {
                Antenna = new AntennaSettings
                {
                    AntennaId = $"ANT-{_counter:000}",
                    Tuning =
                    {
                        new SettingValue { Key = "Azimuth", Value = (110 + _counter).ToString() },
                        new SettingValue { Key = "Elevation", Value = "15" }
                    },
                    Reference =
                    {
                        new SettingValue { Key = "Type", Value = "Yagi" },
                        new SettingValue { Key = "Polarization", Value = "Vertical" }
                    }
                },
                Rpu = new RpuSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "Bandwidth", Value = "25kHz" },
                        new SettingValue { Key = "Gain", Value = (10 + _counter).ToString() }
                    }
                },
                Detector = new DetectorSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "Threshold", Value = "0.42" }
                    }
                },
                Demodulator = new DemodulatorSettings
                {
                    Sections =
                    {
                        new SettingsSection
                        {
                            Name = "PLL",
                            Values =
                            {
                                new SettingValue { Key = "LockRange", Value = "2.5kHz" },
                                new SettingValue { Key = "LoopGain", Value = "1.1" }
                            }
                        }
                    }
                },
                Decoder = new DecoderSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "Scheme", Value = "Viterbi" }
                    }
                }
            }
        };

        return Task.FromResult(snapshot);
    }

    public Task ApplyAsync(SettingsSnapshot snapshot)
    {
        return Task.CompletedTask;
    }
}
