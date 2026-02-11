using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Settings.Core.Models;
using Settings.Core.Services;
using Settings.Integration.Hardware;

namespace Settings.Integration.Services;

// Minimal template for real hardware integration.
// Replace TODOs with actual adapter/API calls.
public class YourHardwareSettingsSource : SettingsSourceBase
{
    private readonly IRuntimeContextProvider _contextProvider;
    private readonly IReadOnlyDictionary<string, IModeSnapshotCollector> _collectors;

    public YourHardwareSettingsSource(
        IRuntimeContextProvider contextProvider,
        IEnumerable<IModeSnapshotCollector> collectors)
    {
        _contextProvider = contextProvider;
        _collectors = collectors.ToDictionary(
            c => c.Mode,
            c => c,
            StringComparer.OrdinalIgnoreCase);
    }

    public override Task<SettingsSnapshot> GetCurrentAsync()
    {
        var runtime = _contextProvider.GetCurrent();
        if (string.IsNullOrWhiteSpace(runtime.Mode))
        {
            throw new InvalidOperationException("Runtime mode is empty.");
        }

        if (!_collectors.TryGetValue(runtime.Mode, out var collector))
        {
            throw new InvalidOperationException(
                $"No snapshot collector is registered for mode '{runtime.Mode}'.");
        }

        var snapshot = collector.Collect(runtime);
        snapshot.Mode = runtime.Mode;
        snapshot.UpdatedAt = DateTimeOffset.UtcNow;
        if (string.IsNullOrWhiteSpace(snapshot.Name))
        {
            snapshot.Name = $"HW {snapshot.Mode}";
        }

        return Task.FromResult(snapshot);
    }

    protected override Task ApplyParameterAsync(string nodeName, SettingValue parameter, CancellationToken ct)
    {
        // TODO: route parameter write to your hardware API.
        return Task.CompletedTask;
    }

    protected override Task ApplySectionValueAsync(string nodeName, string sectionPath, SettingValue value, CancellationToken ct)
    {
        // TODO: route section value write to your hardware API.
        return Task.CompletedTask;
    }
}
