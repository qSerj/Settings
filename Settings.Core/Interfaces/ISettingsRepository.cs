namespace Settings.Core.Interfaces;

public interface ISettingsRepository
{
    public Task<List<Models.Settings>> GetAllAsync();
    public Task SaveAsync(Models.Settings item);
    Task DeleteAsync(Guid settingsId);
}
