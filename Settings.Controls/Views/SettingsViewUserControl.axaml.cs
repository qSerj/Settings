using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Settings.Controls.ViewModels;
using System.Windows.Input;

namespace Settings.Controls.Views;

public partial class SettingsViewUserControl : UserControl
{
    private bool _hasLoaded;

    public ICommand? LoadCommand => (DataContext as SettingsViewUserControlViewModel)?.LoadCommand;
    public ICommand? SaveSnapshotCommand => (DataContext as SettingsViewUserControlViewModel)?.SaveSnapshotCommand;
    public ICommand? ApplyCommand => (DataContext as SettingsViewUserControlViewModel)?.ApplyCommand;
    public ICommand? DeleteCommand => (DataContext as SettingsViewUserControlViewModel)?.DeleteCommand;

    public SettingsViewUserControl()
    {
        InitializeComponent();
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (_hasLoaded)
        {
            return;
        }

        _hasLoaded = true;
        if (DataContext is SettingsViewUserControlViewModel viewModel)
        {
            _ = viewModel.LoadSnapshotsAsync();
        }
    }

    private void OnSettingsGridPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(this).Properties;
        if (!properties.IsRightButtonPressed)
        {
            return;
        }

        var source = e.Source as Avalonia.Visual;
        var row = source?.FindAncestorOfType<DataGridRow>();
        if (row?.DataContext is SettingsSnapshotViewModel selected &&
            DataContext is SettingsViewUserControlViewModel viewModel)
        {
            SettingsGrid.SelectedItem = selected;
            viewModel.Selected = selected;
            viewModel.ApplyCommand.RaiseCanExecuteChanged();
            viewModel.DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    private void OnRowContextMenuOpened(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewUserControlViewModel viewModel)
        {
            ApplyMenuItem.IsEnabled = false;
            DeleteMenuItem.IsEnabled = false;
            return;
        }

        ApplyMenuItem.IsEnabled = viewModel.ApplyCommand.CanExecute();
        DeleteMenuItem.IsEnabled = viewModel.DeleteCommand.CanExecute();
    }

    private void OnApplyMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewUserControlViewModel viewModel)
        {
            return;
        }

        if (viewModel.ApplyCommand.CanExecute())
        {
            viewModel.ApplyCommand.Execute();
        }
    }

    private void OnDeleteMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewUserControlViewModel viewModel)
        {
            return;
        }

        if (viewModel.DeleteCommand.CanExecute())
        {
            viewModel.DeleteCommand.Execute();
        }
    }
}
