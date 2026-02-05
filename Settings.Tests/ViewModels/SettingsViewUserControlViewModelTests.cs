using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Headless.XUnit;
using Settings.Controls.ViewModels;
using Settings.Core.Interfaces;
using Settings.Core.Models;
using Xunit;

namespace Settings.Tests.ViewModels;

public class SettingsViewUserControlViewModelTests
{
    [AvaloniaFact]
    public async Task ApplyCommand_UpdatesStatusAndSteps()
    {
        var repo = new InMemoryRepository(new[]
        {
            new SettingsSnapshot { Name = "Test", Mode = "Global" }
        });
        var source = new StubSettingsSource();
        var applier = new RecordingApplier();
        var vm = new SettingsViewUserControlViewModel(repo, source, applier);

        await vm.LoadSnapshotsAsync();
        vm.Selected = vm.Settings.First();

        vm.ApplyCommand.Execute();
        await WaitUntilAsync(() => vm.StatusMessage == "Применено");

        Assert.Equal("Применено", vm.StatusMessage);
        Assert.Equal(new[] { "Шаг 1", "Шаг 2" }, vm.ApplySteps.Select(s => s.Title).ToArray());
    }

    private static async Task WaitUntilAsync(Func<bool> predicate)
    {
        var start = DateTime.UtcNow;
        while (!predicate())
        {
            if (DateTime.UtcNow - start > TimeSpan.FromSeconds(2))
            {
                throw new TimeoutException("Condition not met.");
            }

            await Task.Delay(10);
        }
    }

    private sealed class InMemoryRepository : ISettingsRepository
    {
        private readonly List<SettingsSnapshot> _items;

        public InMemoryRepository(IEnumerable<SettingsSnapshot> items)
        {
            _items = items.ToList();
        }

        public Task<IEnumerable<SettingsSnapshot>> GetAllAsync() =>
            Task.FromResult<IEnumerable<SettingsSnapshot>>(_items.ToList());

        public Task<SettingsSnapshot?> GetByIdAsync(Guid id) =>
            Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<SettingsSnapshot> SaveAsync(SettingsSnapshot item)
        {
            _items.Add(item);
            return Task.FromResult(item);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var existing = _items.FirstOrDefault(x => x.Id == id);
            if (existing == null) return Task.FromResult(false);
            _items.Remove(existing);
            return Task.FromResult(true);
        }
    }

    private sealed class StubSettingsSource : ISettingsSource
    {
        public Task<SettingsSnapshot> GetCurrentAsync() =>
            Task.FromResult(new SettingsSnapshot());

        public Task ApplyAsync(SettingsSnapshot snapshot) =>
            Task.CompletedTask;
    }

    private sealed class RecordingApplier : ISettingsApplier
    {
        public Task<ApplyResult> ApplyAsync(
            SettingsSnapshot snapshot,
            IApplyReporter reporter,
            CancellationToken ct)
        {
            reporter.StepStarted("Шаг 1");
            reporter.StepSucceeded("Шаг 1");
            reporter.StepStarted("Шаг 2");
            reporter.StepSucceeded("Шаг 2");

            return Task.FromResult(ApplyResult.Ok());
        }
    }
}
