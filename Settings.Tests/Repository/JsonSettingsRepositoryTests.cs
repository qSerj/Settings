using Settings.Core.Models;
using Settings.Core.Services;
using Xunit;

namespace Settings.Tests.Repository;

public class JsonSettingsRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_UsesSampleWhenFileMissing()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"settings-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            EnsureSampleFile();

            var filePath = Path.Combine(tempDir, "settings_snapshots.json");
            var repo = new JsonSettingsRepository(filePath);

            var items = (await repo.GetAllAsync()).ToList();

            Assert.NotEmpty(items);
            Assert.True(File.Exists(filePath));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task SaveAndDelete_RoundTrip()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"settings-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var filePath = Path.Combine(tempDir, "settings_snapshots.json");
            await File.WriteAllTextAsync(filePath, "[]");

            var repo = new JsonSettingsRepository(filePath);
            var snapshot = new SettingsSnapshot
            {
                Name = "Test",
                Mode = "Global",
                Radio = new RadioSettings
                {
                    Rpu = new RpuSettings
                    {
                        Parameters = { new SettingValue { Key = "Gain", Value = "10" } }
                    }
                }
            };

            var saved = await repo.SaveAsync(snapshot);
            var items = (await repo.GetAllAsync()).ToList();

            Assert.Single(items);
            Assert.Equal(saved.Id, items[0].Id);

            var deleted = await repo.DeleteAsync(saved.Id);
            var remaining = (await repo.GetAllAsync()).ToList();

            Assert.True(deleted);
            Assert.Empty(remaining);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    private static void EnsureSampleFile()
    {
        var assetsDir = Path.Combine(AppContext.BaseDirectory, "assets");
        Directory.CreateDirectory(assetsDir);

        var samplePath = Path.Combine(assetsDir, "settings_snapshots.json");
        if (File.Exists(samplePath)) return;

        var sample = """
                     [
                       {
                         "Id": "00000000-0000-0000-0000-000000000010",
                         "Name": "Sample",
                         "Mode": "Global",
                         "CreatedAt": "2026-02-01T08:00:00+00:00",
                         "UpdatedAt": "2026-02-01T08:00:00+00:00",
                         "Radio": {
                           "Rpu": {
                             "IsPresent": true,
                             "IsRelevant": true,
                             "Parameters": [],
                             "Sections": []
                           }
                         }
                       }
                     ]
                     """;

        File.WriteAllText(samplePath, sample);
    }
}
