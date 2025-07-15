namespace Settings.Controls.ViewModels;

public class SettingsViewModel : BindableBase
{
    private Core.Models.Settings _setings;

    public SettingsViewModel(Core.Models.Settings setings)
    {
        _setings = setings;
    }

    public string Type
    {
        get => _setings.Type;
    }
}