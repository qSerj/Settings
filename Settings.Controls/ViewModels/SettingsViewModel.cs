namespace Settings.Controls.ViewModels;

public class SettingsViewModel : BindableBase
{
    private Core.Models.ThingsToSave _setings;

    public SettingsViewModel(Core.Models.ThingsToSave setings)
    {
        _setings = setings;
    }

    public string Type
    {
        get => _setings.Type;
    }
}