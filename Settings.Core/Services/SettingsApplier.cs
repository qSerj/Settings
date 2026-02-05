using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Core.Services;

public class SettingsApplier : ISettingsApplier
{
    private readonly IReadOnlyDictionary<string, ISettingsApplyStrategy> _strategies;

    public SettingsApplier(IEnumerable<ISettingsApplyStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(
            s => s.Mode,
            s => s,
            StringComparer.OrdinalIgnoreCase);
    }

    public Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct)
    {
        if (!_strategies.TryGetValue(snapshot.Mode, out var strategy))
        {
            reporter.StepFailed("Подготовка", $"Не найдена стратегия для режима '{snapshot.Mode}'.");
            return Task.FromResult(ApplyResult.Failed($"Нет стратегии для режима '{snapshot.Mode}'.", "Подготовка"));
        }

        return strategy.ApplyAsync(snapshot, reporter, ct);
    }
}
