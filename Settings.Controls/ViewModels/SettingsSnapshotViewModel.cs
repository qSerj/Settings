using System;
using System.Linq;
using Prism.Mvvm;
using Settings.Core.Models;

namespace Settings.Controls.ViewModels;

public class SettingsSnapshotViewModel : BindableBase
{
    public SettingsSnapshot Model { get; }

    public SettingsSnapshotViewModel(SettingsSnapshot snapshot)
    {
        Model = snapshot;
    }

    public string Name => Model.Name;
    public string Mode => Model.Mode;
    public DateTimeOffset UpdatedAt => Model.UpdatedAt;

    public string SectionsSummary
    {
        get
        {
            var radio = Model.Radio ?? new RadioSettings();
            var entries = new[]
            {
                BuildEntry("Antenna", radio.Antenna),
                BuildEntry("Rpu", radio.Rpu),
                BuildEntry("Detector", radio.Detector),
                BuildEntry("Demodulator", radio.Demodulator),
                BuildEntry("Decoder", radio.Decoder)
            }.Where(e => !string.IsNullOrEmpty(e)).ToList();

            if (entries.Count == 0) return "—";
            return string.Join("; ", entries);
        }
    }

    private static string? BuildEntry(string name, SettingsBlock? block)
    {
        if (block == null || !block.IsPresent || !block.IsRelevant) return null;
        return $"{name}: {CountBlockValues(block)}";
    }

    private static int CountBlockValues(SettingsBlock block)
    {
        var count = block.Parameters.Count;
        foreach (var section in block.Sections)
        {
            count += CountSectionValues(section);
        }

        if (block is AntennaSettings antenna)
        {
            count += antenna.Tuning.Count;
            count += antenna.Reference.Count;
        }

        return count;
    }

    private static int CountSectionValues(SettingsSection section)
    {
        var count = section.Values.Count;
        foreach (var child in section.Sections)
        {
            count += CountSectionValues(child);
        }

        return count;
    }
}
