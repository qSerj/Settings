using Settings.Core.Models;

namespace Settings.Core.Interfaces;

public interface ISettingsRepository
{
    public Task<IEnumerable<ThingsToSave>> GetAllAsync();
    public Task<ThingsToSave> SaveAsync(ThingsToSave item);
    Task<bool> DeleteAsync(Guid settingsId);
}
