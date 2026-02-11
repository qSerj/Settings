using Lamar;
using Settings.Core.Interfaces;
using Settings.Core.Services;
using Settings.Integration.Hardware;
using Settings.Integration.Hardware.Legacy;
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
        services.For<IChannelTargetContext>().Use<AsyncLocalChannelTargetContext>().Singleton();
        services.For<IChannelTargetResolver>().Use<DefaultChannelTargetResolver>().Singleton();
        services.For<ISettingsApplyFanOut>().Use<SettingsApplyFanOut>().Singleton();
        return services;
    }

    public static ServiceRegistry AddMockSettingsSource(this ServiceRegistry services)
    {
        services.For<ISettingsSource>().Use<MockSettingsSource>().Singleton();
        return services;
    }

    public static ServiceRegistry AddYourHardwareSettingsSource(this ServiceRegistry services)
    {
        services.For<IRuntimeContextProvider>().Use<DefaultRuntimeContextProvider>().Singleton();
        services.For<IModeSnapshotCollector>().Add<GlobalModeSnapshotCollector>().Singleton();
        services.For<IModeSnapshotCollector>().Add<ObserveModeSnapshotCollector>().Singleton();
        services.For<ISettingsSource>().Use<YourHardwareSettingsSource>().Singleton();
        return services;
    }

    public static ServiceRegistry UseLegacyRuntimeAdapters(this ServiceRegistry services)
    {
        services.For<IRuntimeContextProvider>().Use<LegacyRuntimeContextProvider>().Singleton();
        services.For<IChannelTargetResolver>().Use<LegacyChannelTargetResolver>().Singleton();
        return services;
    }
}
