using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Threading;
using Prism.Commands;
using Prism.Mvvm;
using Settings.Core.Interfaces;
using Settings.Core.Models;

namespace Settings.Controls.ViewModels;

public class SettingsViewUserControlViewModel : BindableBase
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ISettingsSource _settingsSource;
    private readonly ISettingsApplier _settingsApplier;
    private readonly ObservableCollection<SettingsSnapshotViewModel> _settings;
    private readonly ObservableCollection<ApplyStepViewModel> _applySteps;
    private SettingsSnapshotViewModel? _selected;
    private bool _isBusy;
    private string _statusMessage = string.Empty;

    public ReadOnlyObservableCollection<SettingsSnapshotViewModel> Settings { get; }
    public ReadOnlyObservableCollection<ApplyStepViewModel> ApplySteps { get; }
    public DelegateCommand LoadCommand { get; }
    public DelegateCommand SaveSnapshotCommand { get; }
    public DelegateCommand ApplyCommand { get; }
    public DelegateCommand DeleteCommand { get; }

    public SettingsViewUserControlViewModel(
        ISettingsRepository settingsRepository,
        ISettingsSource settingsSource,
        ISettingsApplier settingsApplier)
    {
        _settingsRepository = settingsRepository;
        _settingsSource = settingsSource;
        _settingsApplier = settingsApplier;
        _settings = new ObservableCollection<SettingsSnapshotViewModel>();
        Settings = new ReadOnlyObservableCollection<SettingsSnapshotViewModel>(_settings);
        _applySteps = new ObservableCollection<ApplyStepViewModel>();
        ApplySteps = new ReadOnlyObservableCollection<ApplyStepViewModel>(_applySteps);

        LoadCommand = new DelegateCommand(async () => await LoadSnapshotsAsync());
        SaveSnapshotCommand = new DelegateCommand(async () => await SaveSnapshotAsync());
        ApplyCommand = new DelegateCommand(async () => await ApplySelectedAsync(), CanApplyOrDelete);
        DeleteCommand = new DelegateCommand(async () => await DeleteSelectedAsync(), CanApplyOrDelete);

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
            await RunOnUiAsync(() =>
            {
                IsBusy = true;
                StatusMessage = "Загрузка...";
            });

            var snapshots = await _settingsRepository.GetAllAsync();
            await RunOnUiAsync(() =>
            {
                _settings.Clear();

                foreach (var snapshot in snapshots)
                {
                    var viewModel = new SettingsSnapshotViewModel(snapshot);
                    viewModel.PropertyChanged += OnPropertyChanged;
                    _settings.Add(viewModel);
                }

                RaisePropertyChanged(nameof(Settings));
                StatusMessage = $"Загружено: {_settings.Count}";
            });
        }
        catch (Exception ex)
        {
            await RunOnUiAsync(() => StatusMessage = $"Ошибка загрузки: {ex.Message}");
        }
        finally
        {
            await RunOnUiAsync(() => IsBusy = false);
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
            await RunOnUiAsync(() =>
            {
                IsBusy = true;
                StatusMessage = "Снятие текущих настроек...";
            });

            var snapshot = await _settingsSource.GetCurrentAsync();
            snapshot.UpdatedAt = DateTimeOffset.UtcNow;

            var saved = await _settingsRepository.SaveAsync(snapshot);
            var viewModel = new SettingsSnapshotViewModel(saved);
            await RunOnUiAsync(() =>
            {
                _settings.Insert(0, viewModel);
                Selected = viewModel;
                StatusMessage = "Снимок сохранен";
            });
        }
        catch (Exception ex)
        {
            await RunOnUiAsync(() => StatusMessage = $"Ошибка сохранения: {ex.Message}");
        }
        finally
        {
            await RunOnUiAsync(() => IsBusy = false);
        }
    }

    private async Task ApplySelectedAsync()
    {
        if (Selected == null) return;

        try
        {
            await RunOnUiAsync(() =>
            {
                IsBusy = true;
                StatusMessage = "Применение...";
                _applySteps.Clear();
            });

            var reporter = new UiApplyReporter(_applySteps, RunOnUiAsync);
            var result = await _settingsApplier.ApplyAsync(
                Selected.Model,
                reporter,
                CancellationToken.None);

            await RunOnUiAsync(() =>
            {
                StatusMessage = result.Success
                    ? "Применено"
                    : $"Ошибка применения: {result.Error}";
            });
        }
        catch (Exception ex)
        {
            await RunOnUiAsync(() => StatusMessage = $"Ошибка применения: {ex.Message}");
        }
        finally
        {
            await RunOnUiAsync(() => IsBusy = false);
        }
    }

    private async Task DeleteSelectedAsync()
    {
        if (Selected == null) return;

        try
        {
            await RunOnUiAsync(() =>
            {
                IsBusy = true;
                StatusMessage = "Удаление...";
            });

            var deleted = await _settingsRepository.DeleteAsync(Selected.Model.Id);
            if (deleted)
            {
                await RunOnUiAsync(() =>
                {
                    _settings.Remove(Selected);
                    Selected = null;
                    StatusMessage = "Удалено";
                });
            }
            else
            {
                await RunOnUiAsync(() => StatusMessage = "Не найдено");
            }
        }
        catch (Exception ex)
        {
            await RunOnUiAsync(() => StatusMessage = $"Ошибка удаления: {ex.Message}");
        }
        finally
        {
            await RunOnUiAsync(() => IsBusy = false);
        }
    }

    private static DispatcherOperation RunOnUiAsync(Action action) =>
        Dispatcher.UIThread.InvokeAsync(action);

    private sealed class UiApplyReporter : IApplyReporter
    {
        private readonly ObservableCollection<ApplyStepViewModel> _steps;
        private readonly Func<Action, DispatcherOperation> _ui;
        private readonly Dictionary<string, ApplyStepViewModel> _byTitle = new();

        public UiApplyReporter(
            ObservableCollection<ApplyStepViewModel> steps,
            Func<Action, DispatcherOperation> ui)
        {
            _steps = steps;
            _ui = ui;
        }

        public void StepStarted(string title)
        {
            _ = _ui(() =>
            {
                var step = new ApplyStepViewModel(title, "Выполняется");
                _byTitle[title] = step;
                _steps.Add(step);
            });
        }

        public void StepSucceeded(string title)
        {
            _ = _ui(() =>
            {
                if (_byTitle.TryGetValue(title, out var step))
                {
                    step.Status = "Готово";
                    step.Error = null;
                }
            });
        }

        public void StepFailed(string title, string error)
        {
            _ = _ui(() =>
            {
                if (_byTitle.TryGetValue(title, out var step))
                {
                    step.Status = "Ошибка";
                    step.Error = error;
                }
                else
                {
                    var failed = new ApplyStepViewModel(title, "Ошибка") { Error = error };
                    _byTitle[title] = failed;
                    _steps.Add(failed);
                }
            });
        }
    }
}
