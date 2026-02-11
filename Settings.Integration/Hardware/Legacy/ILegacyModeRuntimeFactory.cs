namespace Settings.Integration.Hardware.Legacy;

// Adapter boundary for your "mode => composed runtime object" factory.
public interface ILegacyModeRuntimeFactory
{
    string GetMode(object channel);
    object? BuildModeRuntime(string mode, object channel);
}
