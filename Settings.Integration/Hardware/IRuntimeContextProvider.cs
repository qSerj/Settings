namespace Settings.Integration.Hardware;

public interface IRuntimeContextProvider
{
    RuntimeContext GetCurrent();
}
