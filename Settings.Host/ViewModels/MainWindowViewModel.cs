using System.Collections.ObjectModel;
using Settings.Controls.ViewModels;
using Settings.Core.Services;

namespace Settings.Host;

public class MainWindowViewModel
{
    private readonly SettingsViewUserControlViewModel _view;

    public ReadOnlyObservableCollection<SettingsViewModel> Settings => _view.Settings;

    public SettingsViewUserControlViewModel SettingsView => _view;
    
    public MainWindowViewModel(SettingsViewUserControlViewModel view)
    {
        _view = view;
    }
}