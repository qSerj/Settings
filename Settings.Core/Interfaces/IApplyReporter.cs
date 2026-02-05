namespace Settings.Core.Interfaces;

public interface IApplyReporter
{
    void StepStarted(string title);
    void StepSucceeded(string title);
    void StepFailed(string title, string error);
}
