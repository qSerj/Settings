using Settings.Core.Models;

namespace Settings.Core.Interfaces;

public interface ISettingsSource
{
    Task<SettingsSnapshot> GetCurrentAsync();
    Task ApplyAsync(SettingsSnapshot snapshot);
}
