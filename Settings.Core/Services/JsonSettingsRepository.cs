using System.Text.Json;
using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Core.Services;

public class JsonSettingsRepository : ISettingsRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private List<SettingsSnapshot> _snapshots = new();
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly string _samplePath;

    // Default path points to the user profile, while a sample file is shipped
    // under the assets directory for first-run population.
    public JsonSettingsRepository(string? filePath = null)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appData, "Settings.Host");
        Directory.CreateDirectory(folder);

        _filePath = filePath ?? Path.Combine(folder, "settings_snapshots.json");
        _samplePath = Path.Combine(AppContext.BaseDirectory, "assets", "settings_snapshots.json");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    public async Task<IEnumerable<SettingsSnapshot>> GetAllAsync()
    {
        await LoadIfNeededAsync();
        return _snapshots.ToList();
    }

    public async Task<SettingsSnapshot?> GetByIdAsync(Guid id)
    {
        await LoadIfNeededAsync();
        return _snapshots.FirstOrDefault(d => d.Id == id);
    }

    public async Task<SettingsSnapshot> SaveAsync(SettingsSnapshot snapshot)
    {
        await LoadIfNeededAsync();

        await _fileLock.WaitAsync();
        try
        {
            var existing = _snapshots.FirstOrDefault(d => d.Id == snapshot.Id);
            if (existing != null)
            {
                var index = _snapshots.IndexOf(existing);
                _snapshots[index] = snapshot;
            }
            else
            {
                _snapshots.Add(snapshot);
            }

            await SaveToFileAsync();
            return snapshot;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await LoadIfNeededAsync();

        await _fileLock.WaitAsync();
        try
        {
            var definition = _snapshots.FirstOrDefault(d => d.Id == id);
            if (definition == null) return false;
            _snapshots.Remove(definition);
            await SaveToFileAsync();
            return true;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private async Task LoadIfNeededAsync()
    {
        if (_snapshots.Any()) return;

        await _fileLock.WaitAsync();
        try
        {
            if (_snapshots.Any()) return; // Double-check

            if (!File.Exists(_filePath))
            {
                if (File.Exists(_samplePath))
                {
                    File.Copy(_samplePath, _filePath);
                }
                else
                {
                    _snapshots = new List<SettingsSnapshot>();
                    return;
                }
            }

            var json = await File.ReadAllTextAsync(_filePath);
            _snapshots = JsonSerializer.Deserialize<List<SettingsSnapshot>>(json, _jsonOptions)
                         ?? new List<SettingsSnapshot>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load definitions from {_filePath}", ex);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private async Task SaveToFileAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_snapshots, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save definitions to {_filePath}", ex);
        }
    }
}
