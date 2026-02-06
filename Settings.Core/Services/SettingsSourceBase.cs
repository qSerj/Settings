using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Core.Services;

public abstract class SettingsSourceBase : ISettingsSource
{
    public abstract Task<SettingsSnapshot> GetCurrentAsync();

    public virtual async Task ApplyAsync(SettingsSnapshot snapshot)
    {
        if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));

        var radio = snapshot.Radio;
        if (radio == null) return;

        await ApplyAntennaAsync(radio.Antenna);
        await ApplyBlockAsync(nameof(RadioSettings.Rpu), radio.Rpu);
        await ApplyBlockAsync(nameof(RadioSettings.Detector), radio.Detector);
        await ApplyBlockAsync(nameof(RadioSettings.Demodulator), radio.Demodulator);
        await ApplyBlockAsync(nameof(RadioSettings.Decoder), radio.Decoder);
    }

    protected virtual Task ApplyAntennaIdAsync(string antennaId, CancellationToken ct) =>
        Task.CompletedTask;

    protected virtual Task ApplyAntennaTuningAsync(SettingValue tuning, CancellationToken ct) =>
        Task.CompletedTask;

    protected virtual Task ApplyAntennaReferenceAsync(SettingValue reference, CancellationToken ct) =>
        Task.CompletedTask;

    protected virtual bool ShouldApplyReference(AntennaSettings antenna) => false;

    protected virtual Task ApplyParameterAsync(string nodeName, SettingValue parameter, CancellationToken ct) =>
        Task.CompletedTask;

    protected virtual Task ApplySectionValueAsync(
        string nodeName,
        string sectionPath,
        SettingValue value,
        CancellationToken ct) =>
        Task.CompletedTask;

    protected virtual bool ShouldApply(SettingsBlock block) =>
        block.IsPresent && block.IsRelevant;

    private async Task ApplyAntennaAsync(AntennaSettings? antenna)
    {
        if (antenna == null || !ShouldApply(antenna)) return;

        if (!string.IsNullOrWhiteSpace(antenna.AntennaId))
        {
            await ApplyAntennaIdAsync(antenna.AntennaId!, CancellationToken.None);
        }

        foreach (var tuning in antenna.Tuning)
        {
            await ApplyAntennaTuningAsync(tuning, CancellationToken.None);
        }

        await ApplyParametersAndSectionsAsync(nameof(RadioSettings.Antenna), antenna);

        if (ShouldApplyReference(antenna))
        {
            foreach (var reference in antenna.Reference)
            {
                await ApplyAntennaReferenceAsync(reference, CancellationToken.None);
            }
        }
    }

    private async Task ApplyBlockAsync(string nodeName, SettingsBlock? block)
    {
        if (block == null || !ShouldApply(block)) return;

        await ApplyParametersAndSectionsAsync(nodeName, block);
    }

    private async Task ApplyParametersAndSectionsAsync(string nodeName, SettingsBlock block)
    {
        foreach (var parameter in block.Parameters)
        {
            await ApplyParameterAsync(nodeName, parameter, CancellationToken.None);
        }

        await ApplySectionsAsync(nodeName, block.Sections, string.Empty);
    }

    private async Task ApplySectionsAsync(
        string nodeName,
        IReadOnlyList<SettingsSection> sections,
        string parentPath)
    {
        foreach (var section in sections)
        {
            var name = string.IsNullOrWhiteSpace(section.Name) ? "Section" : section.Name.Trim();
            var path = string.IsNullOrEmpty(parentPath) ? name : $"{parentPath}/{name}";

            foreach (var value in section.Values)
            {
                await ApplySectionValueAsync(nodeName, path, value, CancellationToken.None);
            }

            await ApplySectionsAsync(nodeName, section.Sections, path);
        }
    }
}
