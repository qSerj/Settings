using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Settings.Controls.ViewModels;

namespace Settings.Controls.Views;

public partial class SettingsViewUserControl : UserControl
{
    private bool _hasLoaded;

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
}
