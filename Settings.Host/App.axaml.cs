using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Lamar;
using Settings.Controls.ViewModels;
using Settings.Core.Interfaces;
using Settings.Core.Services;
using Settings.Host.Services;
using Settings.Host.Views;
using Serilog;

namespace Settings.Host;

public partial class App : Application
{
    public static IContainer Container { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ConfigureLogging();

        Container = new Container(x =>
        {
            x.For<ISettingsRepository>().Use<JsonSettingsRepository>().Singleton();
            x.For<ISettingsSource>().Use<MockSettingsSource>().Singleton();
            x.For<SettingsApplier>().Use<SettingsApplier>().Singleton();
            x.For<ISettingsApplier>().Use<LoggingSettingsApplier>().Singleton();
            x.For<ISettingsApplyStrategy>().Add<GlobalModeApplyStrategy>().Singleton();
            x.For<ISettingsApplyStrategy>().Add<ObserveModeApplyStrategy>().Singleton();
            x.For<MainWindowViewModel>().Use<MainWindowViewModel>().Transient();
            x.For<SettingsViewUserControlViewModel>().Use<SettingsViewUserControlViewModel>().Transient();
        });

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.DataContext = Container.GetInstance<MainWindowViewModel>();
            desktop.Exit += (_, _) => Log.CloseAndFlush();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureLogging()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var logFolder = Path.Combine(appData, "Settings.Host", "logs");
        Directory.CreateDirectory(logFolder);
        var logFilePath = Path.Combine(logFolder, "settings-apply-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}
