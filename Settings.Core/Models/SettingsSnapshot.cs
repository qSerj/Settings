namespace Settings.Core.Models;

public class SettingsSnapshot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Снимок";
    public string Mode { get; set; } = "Global";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<SettingsSection> Sections { get; set; } = new();
}

public class SettingsSection
{
    public string Name { get; set; } = string.Empty;
    public List<SettingValue> Values { get; set; } = new();
}

public class SettingValue
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; } = string.Empty;
}
