namespace Settings.Core.Models;

public class ApplyResult
{
    public bool Success { get; }
    public string? Error { get; }
    public string? LastStep { get; }

    private ApplyResult(bool success, string? error, string? lastStep)
    {
        Success = success;
        Error = error;
        LastStep = lastStep;
    }

    public static ApplyResult Ok() => new(true, null, null);

    public static ApplyResult Failed(string error, string? lastStep = null) =>
        new(false, error, lastStep);
}
