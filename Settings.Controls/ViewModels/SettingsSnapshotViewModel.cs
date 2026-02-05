using System;
using System.Linq;
using Prism.Mvvm;
using Settings.Core.Models;

namespace Settings.Controls.ViewModels;

public class SettingsSnapshotViewModel : BindableBase
{
    public SettingsSnapshot Model { get; }

    public SettingsSnapshotViewModel(SettingsSnapshot snapshot)
    {
        Model = snapshot;
    }

    public string Name => Model.Name;
    public string Mode => Model.Mode;
    public DateTimeOffset UpdatedAt => Model.UpdatedAt;

    public string AntennaSummary => BuildAntennaSummary();
    public string RpuSummary => BuildRpuSummary();
    public string DetectorSummary => BuildDetectorSummary();
    public string DemodulatorSummary => BuildDemodulatorSummary();
    public string DecoderSummary => BuildDecoderSummary();

    public string SectionsSummary
    {
        get
        {
            var radio = Model.Radio ?? new RadioSettings();
            var entries = new[]
            {
                BuildEntry("Antenna", radio.Antenna),
                BuildEntry("Rpu", radio.Rpu),
                BuildEntry("Detector", radio.Detector),
                BuildEntry("Demodulator", radio.Demodulator),
                BuildEntry("Decoder", radio.Decoder)
            }.Where(e => !string.IsNullOrEmpty(e)).ToList();

            if (entries.Count == 0) return "—";
            return string.Join("; ", entries);
        }
    }

    private string BuildAntennaSummary()
    {
        var antenna = Model.Radio?.Antenna;
        if (antenna == null || !antenna.IsPresent || !antenna.IsRelevant) return "—";

        var name = GetValue(antenna.Reference, "Антенна")
                   ?? GetValue(antenna.Parameters, "Антенна");
        var mspr = GetValue(antenna.Reference, "МШПР");
        var range = GetValue(antenna.Reference, "Диапазон");
        var pol = GetValue(antenna.Reference, "Поляризация");

        if (!string.IsNullOrWhiteSpace(mspr) && !string.IsNullOrWhiteSpace(name))
        {
            name = $"{name} ({mspr})";
        }

        return JoinParts(name, range, pol);
    }

    private string BuildRpuSummary()
    {
        var rpu = Model.Radio?.Rpu;
        if (rpu == null || !rpu.IsPresent || !rpu.IsRelevant) return "—";

        var freq = GetValue(rpu.Parameters, "Частота приемника, кГц");
        var att = GetValue(rpu.Parameters, "Аттенюатор");
        var overload = GetValue(rpu.Parameters, "Перегруз АЦП");

        var freqPart = string.IsNullOrWhiteSpace(freq) ? null : $"Fпр {freq} кГц";
        var attPart = string.IsNullOrWhiteSpace(att) ? null : $"Атт {att}";
        var overloadPart = string.IsNullOrWhiteSpace(overload) ? null : $"Перегруз {overload}";

        return JoinParts(freqPart, attPart, overloadPart);
    }

    private string BuildDetectorSummary()
    {
        var detector = Model.Radio?.Detector;
        if (detector == null || !detector.IsPresent || !detector.IsRelevant) return "—";

        var carrier = GetValue(detector.Parameters, "Несущая частота, кГц");
        var clock = GetValue(detector.Parameters, "Тактовая частота, кГц");
        var afc = GetValue(detector.Parameters, "ФАПЧ");
        var filter = GetValue(detector.Parameters, "Фильтр");

        var carrierPart = string.IsNullOrWhiteSpace(carrier) ? null : $"Нос {carrier} кГц";
        var clockPart = string.IsNullOrWhiteSpace(clock) ? null : $"Такт {clock} кГц";
        var afcPart = string.IsNullOrWhiteSpace(afc) ? null : $"ФАПЧ {afc}";
        var filterPart = string.IsNullOrWhiteSpace(filter) ? null : $"Фильтр {filter}";

        return JoinParts(carrierPart, clockPart, afcPart, filterPart);
    }

    private string BuildDemodulatorSummary()
    {
        var demodulator = Model.Radio?.Demodulator;
        if (demodulator == null || !demodulator.IsPresent || !demodulator.IsRelevant) return "—";

        var coder = GetValue(demodulator.Parameters, "Кодер");
        var modulation = GetValue(demodulator.Parameters, "Модуляция");
        var pilots = GetValue(demodulator.Parameters, "Пилоты");
        var inversion = GetValue(demodulator.Parameters, "Инверсия спектра");
        var snr = GetValue(demodulator.Parameters, "ОСШ");

        var coderPart = string.IsNullOrWhiteSpace(coder) ? null : $"Кодер {coder}";
        var modulationPart = string.IsNullOrWhiteSpace(modulation) ? null : modulation;
        var pilotsPart = string.IsNullOrWhiteSpace(pilots) ? null : $"Пилоты {pilots}";
        var inversionPart = string.IsNullOrWhiteSpace(inversion) ? null : $"Инв {inversion}";
        var snrPart = string.IsNullOrWhiteSpace(snr) ? null : $"ОСШ {snr}";

        return JoinParts(coderPart, modulationPart, pilotsPart, inversionPart, snrPart);
    }

    private string BuildDecoderSummary()
    {
        var decoder = Model.Radio?.Decoder;
        if (decoder == null || !decoder.IsPresent || !decoder.IsRelevant) return "—";

        var coder = GetValue(decoder.Parameters, "Кодер");
        var iterations = GetValue(decoder.Parameters, "Итераций");
        var temperature = GetValue(decoder.Parameters, "Температура");
        var signature = GetValue(decoder.Parameters, "Сигнатура");

        var coderPart = string.IsNullOrWhiteSpace(coder) ? null : $"Кодер {coder}";
        var iterationsPart = string.IsNullOrWhiteSpace(iterations) ? null : $"Ит {iterations}";
        var temperaturePart = string.IsNullOrWhiteSpace(temperature) ? null : $"T {temperature}";
        var signaturePart = string.IsNullOrWhiteSpace(signature) ? null : $"Сигн {signature}";

        return JoinParts(coderPart, iterationsPart, temperaturePart, signaturePart);
    }

    private static string JoinParts(params string?[] parts)
    {
        var filtered = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
        return filtered.Length == 0 ? "—" : string.Join(" · ", filtered);
    }

    private static string? GetValue(IEnumerable<SettingValue> values, string key)
    {
        return values.FirstOrDefault(v => v.Key == key)?.Value;
    }

    private static string? BuildEntry(string name, SettingsBlock? block)
    {
        if (block == null || !block.IsPresent || !block.IsRelevant) return null;
        return $"{name}: {CountBlockValues(block)}";
    }

    private static int CountBlockValues(SettingsBlock block)
    {
        var count = block.Parameters.Count;
        foreach (var section in block.Sections)
        {
            count += CountSectionValues(section);
        }

        if (block is AntennaSettings antenna)
        {
            count += antenna.Tuning.Count;
            count += antenna.Reference.Count;
        }

        return count;
    }

    private static int CountSectionValues(SettingsSection section)
    {
        var count = section.Values.Count;
        foreach (var child in section.Sections)
        {
            count += CountSectionValues(child);
        }

        return count;
    }
}
