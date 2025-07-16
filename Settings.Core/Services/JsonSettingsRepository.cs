using System.Text.Json;
using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Core.Services;

public class JsonSettingsRepository :  ISettingsRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private List<ThingsToSave> _things = new();
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public JsonSettingsRepository(string filePath = "assets/antenna_definitions.json")
    {
        _filePath = filePath;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    public async Task<IEnumerable<Models.ThingsToSave>> GetAllAsync()
    {
        await LoadIfNeededAsync();
        return _things.ToList();
    }

    public async Task<ThingsToSave?> GetByIdAsync(Guid id)
    {
        await LoadIfNeededAsync();
        return _things.FirstOrDefault(d => d.Id == id);
    }

    public async Task<ThingsToSave> SaveAsync(ThingsToSave things)
    {
        await LoadIfNeededAsync();
        
        await _fileLock.WaitAsync();
        try
        {
            var existing = _things.FirstOrDefault(d => d.Id == things.Id);
            if (existing != null)
            {
                var index = _things.IndexOf(existing);
                _things[index] = things;
            }
            else
            {
                _things.Add(things);
            }
            
            await SaveToFileAsync();
            return things;
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
            var definition = _things.FirstOrDefault(d => d.Id == id);
            if (definition == null) return false;
            _things.Remove(definition);
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
        if (_things.Any()) return;
        
        await _fileLock.WaitAsync();
        try
        {
            if (_things.Any()) return; // Double-check
            
            if (!File.Exists(_filePath))
            {
                _things = new List<ThingsToSave>();
                return;
            }

            var json = await File.ReadAllTextAsync(_filePath);
            _things = JsonSerializer.Deserialize<List<ThingsToSave>>(json, _jsonOptions) 
                      ?? new List<ThingsToSave>();
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
            var json = JsonSerializer.Serialize(_things, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save definitions to {_filePath}", ex);
        }
    }

}