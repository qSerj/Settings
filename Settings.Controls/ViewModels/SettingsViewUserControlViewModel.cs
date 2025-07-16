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
            var antennas = await _settingsRepository.GetAllAsync();

            _settings.Clear();

            foreach (var ant in antennas)
            {
                var viewModel =
                    new SettingsViewModel(ant);
                viewModel.PropertyChanged += OnPropertyChanged;
                _settings.Add(viewModel);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки антенн: {ex.Message}");
        }
    }
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(e.PropertyName);
    }
}