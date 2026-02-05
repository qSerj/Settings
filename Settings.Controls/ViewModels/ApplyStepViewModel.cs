using Prism.Mvvm;

namespace Settings.Controls.ViewModels;

public class ApplyStepViewModel : BindableBase
{
    private string _status;
    private string? _error;

    public string Title { get; }

    public string Status
    {
        get => _status;
        set
        {
            if (SetProperty(ref _status, value))
            {
                RaisePropertyChanged(nameof(Display));
            }
        }
    }

    public string? Error
    {
        get => _error;
        set
        {
            if (SetProperty(ref _error, value))
            {
                RaisePropertyChanged(nameof(Display));
            }
        }
    }

    public string Display =>
        string.IsNullOrWhiteSpace(Error)
            ? $"{Title} — {Status}"
            : $"{Title} — {Status}: {Error}";

    public ApplyStepViewModel(string title, string status)
    {
        Title = title;
        _status = status;
    }
}
