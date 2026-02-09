using System;
using System.IO;
using Serilog;

namespace Settings.Integration.Logging;

public static class SettingsApplyLoggingConfigurator
{
    public static void ConfigureFileLogging(
        string appName,
        string logFilePrefix = "settings-apply-",
        int retainedFileCountLimit = 14)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var logFolder = Path.Combine(appData, appName, "logs");
        Directory.CreateDirectory(logFolder);
        var logFilePath = Path.Combine(logFolder, $"{logFilePrefix}.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: retainedFileCountLimit,
                shared: true,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static void CloseAndFlush() => Log.CloseAndFlush();
}
