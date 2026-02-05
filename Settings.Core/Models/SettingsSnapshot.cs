namespace Settings.Core.Models;

public class SettingsSnapshot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Снимок";
    public string Mode { get; set; } = "Global";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public RadioSettings Radio { get; set; } = new();
}

public class RadioSettings
{
    public AntennaSettings? Antenna { get; set; }
    public RpuSettings? Rpu { get; set; }
    public DetectorSettings? Detector { get; set; }
    public DemodulatorSettings? Demodulator { get; set; }
    public DecoderSettings? Decoder { get; set; }
}

public class SettingsBlock
{
    public bool IsPresent { get; set; } = true;
    public bool IsRelevant { get; set; } = true;
    public List<SettingValue> Parameters { get; set; } = new();
    public List<SettingsSection> Sections { get; set; } = new();
}

public class AntennaSettings : SettingsBlock
{
    public string? AntennaId { get; set; }
    public List<SettingValue> Tuning { get; set; } = new();
    public List<SettingValue> Reference { get; set; } = new();
}

public class RpuSettings : SettingsBlock
{
}

public class DetectorSettings : SettingsBlock
{
}

public class DemodulatorSettings : SettingsBlock
{
}

public class DecoderSettings : SettingsBlock
{
}

public class SettingsSection
{
    public string Name { get; set; } = string.Empty;
    public List<SettingValue> Values { get; set; } = new();
    public List<SettingsSection> Sections { get; set; } = new();
}

public class SettingValue
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; } = string.Empty;
}
