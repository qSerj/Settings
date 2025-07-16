namespace Settings.Core.Models;

public class ThingsToSave
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Type { get; set; }
    public double S1 { get; set;}
    public double S2 { get; set;}
}
