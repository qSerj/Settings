using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Lamar;
using Settings.Controls.ViewModels;
using Settings.Core.Interfaces;
using Settings.Core.Services;
using Settings.Host.Services;
using Settings.Host.Views;

namespace Settings.Host;

public partial class App : Application
{
    public static IContainer Container { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Container = new Container(x =>
        {
            x.For<ISettingsRepository>().Use<JsonSettingsRepository>().Singleton();
            x.For<ISettingsSource>().Use<MockSettingsSource>().Singleton();
            x.For<ISettingsApplier>().Use<SettingsApplier>().Singleton();
            x.For<ISettingsApplyStrategy>().Add<GlobalModeApplyStrategy>().Singleton();
            x.For<ISettingsApplyStrategy>().Add<ObserveModeApplyStrategy>().Singleton();
            x.For<MainWindowViewModel>().Use<MainWindowViewModel>().Transient();
            x.For<SettingsViewUserControlViewModel>().Use<SettingsViewUserControlViewModel>().Transient();
        });

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.DataContext = Container.GetInstance<MainWindowViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
