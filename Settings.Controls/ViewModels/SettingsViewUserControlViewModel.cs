using System.Collections.ObjectModel;
using Settings.Core.Interfaces;

namespace Settings.Controls.ViewModels;

public class SettingsViewUserControlViewModel : BindableBase
{
    private ISettingsRepository _settingsRepository;
    private ObservableCollection<SettingsViewModel> _settings;
    public ReadOnlyObservableCollection<SettingsViewModel> Settings { get; }

    public SettingsViewUserControlViewModel(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
        _settings = new ObservableCollection<SettingsViewModel>(_settingsRepository.GetAllAsync());
        Settings = new ReadOnlyObservableCollection<SettingsViewModel>(_settings);
    }
}