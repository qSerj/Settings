using System.Collections.ObjectModel;
using Prism.Commands;
using Settings.Controls.ViewModels;

namespace Settings.Host;

public class MainWindowViewModel
{
    private readonly SettingsViewUserControlViewModel _view;

    public ReadOnlyObservableCollection<SettingsSnapshotViewModel> Settings => _view.Settings;

    public SettingsViewUserControlViewModel SettingsView => _view;
    public DelegateCommand LoadCommand => _view.LoadCommand;
    public DelegateCommand SaveSnapshotCommand => _view.SaveSnapshotCommand;
    public DelegateCommand ApplyCommand => _view.ApplyCommand;
    public DelegateCommand DeleteCommand => _view.DeleteCommand;

    public MainWindowViewModel(SettingsViewUserControlViewModel view)
    {
        _view = view;
    }
}
