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

    public double S1
    {
        get => _setings.S1;
    }

    public double S2
    {
        get => _setings.S2;
    }
}