using System;
using System.Threading;
using System.Threading.Tasks;
using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Host.Services;

public class ObserveModeApplyStrategy : ISettingsApplyStrategy
{
    private readonly ISettingsSource _settingsSource;

    public string Mode => "Observe";

    public ObserveModeApplyStrategy(ISettingsSource settingsSource)
    {
        _settingsSource = settingsSource;
    }

    public async Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct)
    {
        const string stepInit = "Подготовка";
        const string stepApply = "Применение мониторинга";

        try
        {
            reporter.StepStarted(stepInit);
            await Task.Yield();
            reporter.StepSucceeded(stepInit);

            reporter.StepStarted(stepApply);
            await _settingsSource.ApplyAsync(snapshot);
            reporter.StepSucceeded(stepApply);

            return ApplyResult.Ok();
        }
        catch (Exception ex)
        {
            reporter.StepFailed(stepApply, ex.Message);
            return ApplyResult.Failed(ex.Message, stepApply);
        }
    }
}
