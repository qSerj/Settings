using System.Collections.ObjectModel;
using Settings.Controls.ViewModels;

namespace Settings.Host;

public class MainWindowViewModel
{
    private readonly SettingsViewUserControlViewModel _view;

    public ReadOnlyObservableCollection<SettingsSnapshotViewModel> Settings => _view.Settings;

    public SettingsViewUserControlViewModel SettingsView => _view;

    public MainWindowViewModel(SettingsViewUserControlViewModel view)
    {
        _view = view;
    }
}
