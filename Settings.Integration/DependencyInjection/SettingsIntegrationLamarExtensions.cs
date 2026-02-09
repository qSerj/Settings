using Lamar;
using Settings.Core.Interfaces;
using Settings.Core.Services;
using Settings.Integration.Services;

namespace Settings.Integration.DependencyInjection;

public static class SettingsIntegrationLamarExtensions
{
    public static ServiceRegistry AddSettingsIntegration(this ServiceRegistry services)
    {
        services.For<SettingsApplier>().Use<SettingsApplier>().Singleton();
        services.For<ISettingsApplier>().Use<LoggingSettingsApplier>().Singleton();
        services.For<ISettingsApplyStrategy>().Add<GlobalModeApplyStrategy>().Singleton();
        services.For<ISettingsApplyStrategy>().Add<ObserveModeApplyStrategy>().Singleton();
        return services;
    }
}
