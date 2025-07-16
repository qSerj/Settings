using System.Collections.ObjectModel;
using System.ComponentModel;
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
        _settings = new ObservableCollection<SettingsViewModel>();
        Settings = new ReadOnlyObservableCollection<SettingsViewModel>(_settings);

        _ = LoadThings();
    }

    public async Task LoadThings()
    {
        try
        {
            var things = await _settingsRepository.GetAllAsync();

            _settings.Clear();

            foreach (var t in things)
            {
                var viewModel =
                    new SettingsViewModel(t);
                viewModel.PropertyChanged += OnPropertyChanged;
                _settings.Add(viewModel);
            }
            RaisePropertyChanged(nameof(Settings));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки: {ex.Message}");
        }
    }
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(e.PropertyName);
    }
}