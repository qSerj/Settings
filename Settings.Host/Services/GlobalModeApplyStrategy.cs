using System;
using System.Threading;
using System.Threading.Tasks;
using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Host.Services;

public class GlobalModeApplyStrategy : ISettingsApplyStrategy
{
    private readonly ISettingsSource _settingsSource;

    public string Mode => "Global";

    public GlobalModeApplyStrategy(ISettingsSource settingsSource)
    {
        _settingsSource = settingsSource;
    }

    public async Task<ApplyResult> ApplyAsync(SettingsSnapshot snapshot, IApplyReporter reporter, CancellationToken ct)
    {
        const string stepInit = "Подготовка";
        const string stepApply = "Применение настроек";
        const string stepFinalize = "Завершение";

        try
        {
            reporter.StepStarted(stepInit);
            await Task.Yield();
            reporter.StepSucceeded(stepInit);

            reporter.StepStarted(stepApply);
            await _settingsSource.ApplyAsync(snapshot);
            reporter.StepSucceeded(stepApply);

            reporter.StepStarted(stepFinalize);
            await Task.Yield();
            reporter.StepSucceeded(stepFinalize);

            return ApplyResult.Ok();
        }
        catch (Exception ex)
        {
            reporter.StepFailed(stepApply, ex.Message);
            return ApplyResult.Failed(ex.Message, stepApply);
        }
    }
}
