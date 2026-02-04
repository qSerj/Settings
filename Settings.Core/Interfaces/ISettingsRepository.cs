using Settings.Core.Models;

namespace Settings.Core.Interfaces;

public interface ISettingsRepository
{
    Task<IEnumerable<SettingsSnapshot>> GetAllAsync();
    Task<SettingsSnapshot?> GetByIdAsync(Guid id);
    Task<SettingsSnapshot> SaveAsync(SettingsSnapshot item);
    Task<bool> DeleteAsync(Guid id);
}
