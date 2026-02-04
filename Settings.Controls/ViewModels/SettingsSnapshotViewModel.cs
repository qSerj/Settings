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
            if (Model.Sections.Count == 0) return "—";
            return string.Join("; ",
                Model.Sections.Select(s => $"{s.Name}: {s.Values.Count}"));
        }
    }
}
