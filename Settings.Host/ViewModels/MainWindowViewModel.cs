using System.Collections.ObjectModel;
using Settings.Controls.ViewModels;
using Settings.Core.Services;

namespace Settings.Host;

public class MainWindowViewModel
{
    SettingsViewUserControlViewModel? _view;

    public ReadOnlyObservableCollection<SettingsViewModel> Settings { get => _view?.Settings; }
    
    public MainWindowViewModel(SettingsViewUserControlViewModel view)
    {
        _view = view;
    }
}