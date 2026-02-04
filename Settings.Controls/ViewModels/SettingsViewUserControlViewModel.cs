using System.Collections.ObjectModel;
using System.ComponentModel;
using Prism.Commands;
using Prism.Mvvm;
using Settings.Core.Interfaces;

namespace Settings.Controls.ViewModels;

public class SettingsViewUserControlViewModel : BindableBase
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ISettingsSource _settingsSource;
    private readonly ObservableCollection<SettingsSnapshotViewModel> _settings;
    private SettingsSnapshotViewModel? _selected;
    private bool _isBusy;
    private string _statusMessage = string.Empty;

    public ReadOnlyObservableCollection<SettingsSnapshotViewModel> Settings { get; }
    public DelegateCommand LoadCommand { get; }
    public DelegateCommand SaveSnapshotCommand { get; }
    public DelegateCommand ApplyCommand { get; }
    public DelegateCommand DeleteCommand { get; }

    public SettingsViewUserControlViewModel(
        ISettingsRepository settingsRepository,
        ISettingsSource settingsSource)
    {
        _settingsRepository = settingsRepository;
        _settingsSource = settingsSource;
        _settings = new ObservableCollection<SettingsSnapshotViewModel>();
        Settings = new ReadOnlyObservableCollection<SettingsSnapshotViewModel>(_settings);

        LoadCommand = new DelegateCommand(async () => await LoadSnapshotsAsync());
        SaveSnapshotCommand = new DelegateCommand(async () => await SaveSnapshotAsync());
        ApplyCommand = new DelegateCommand(async () => await ApplySelectedAsync(), CanApplyOrDelete);
        DeleteCommand = new DelegateCommand(async () => await DeleteSelectedAsync(), CanApplyOrDelete);

        _ = LoadSnapshotsAsync();
    }

    public SettingsSnapshotViewModel? Selected
    {
        get => _selected;
        set
        {
            if (SetProperty(ref _selected, value))
            {
                ApplyCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public async Task LoadSnapshotsAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Загрузка...";

            var snapshots = await _settingsRepository.GetAllAsync();
            _settings.Clear();

            foreach (var snapshot in snapshots)
            {
                var viewModel = new SettingsSnapshotViewModel(snapshot);
                viewModel.PropertyChanged += OnPropertyChanged;
                _settings.Add(viewModel);
            }

            RaisePropertyChanged(nameof(Settings));
            StatusMessage = $"Загружено: {_settings.Count}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(e.PropertyName);
    }

    private bool CanApplyOrDelete() => Selected != null && !IsBusy;

    private async Task SaveSnapshotAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Снятие текущих настроек...";

            var snapshot = await _settingsSource.GetCurrentAsync();
            snapshot.UpdatedAt = DateTimeOffset.UtcNow;

            var saved = await _settingsRepository.SaveAsync(snapshot);
            var viewModel = new SettingsSnapshotViewModel(saved);
            _settings.Insert(0, viewModel);
            Selected = viewModel;
            StatusMessage = "Снимок сохранен";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сохранения: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ApplySelectedAsync()
    {
        if (Selected == null) return;

        try
        {
            IsBusy = true;
            StatusMessage = "Применение...";

            await _settingsSource.ApplyAsync(Selected.Model);
            StatusMessage = "Применено";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка применения: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteSelectedAsync()
    {
        if (Selected == null) return;

        try
        {
            IsBusy = true;
            StatusMessage = "Удаление...";

            var deleted = await _settingsRepository.DeleteAsync(Selected.Model.Id);
            if (deleted)
            {
                _settings.Remove(Selected);
                Selected = null;
                StatusMessage = "Удалено";
            }
            else
            {
                StatusMessage = "Не найдено";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка удаления: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
