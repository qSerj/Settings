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
            Sections = new List<SettingsSection>
            {
                new()
                {
                    Name = "Антенна",
                    Values = new List<SettingValue>
                    {
                        new() { Key = "Gain", Value = (10 + _counter).ToString() },
                        new() { Key = "Polarization", Value = "Vertical" }
                    }
                },
                new()
                {
                    Name = "Передатчик",
                    Values = new List<SettingValue>
                    {
                        new() { Key = "Power", Value = $"{5 + _counter}W" },
                        new() { Key = "Frequency", Value = "145.000MHz" }
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
