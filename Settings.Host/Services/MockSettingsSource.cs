using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Settings.Core.Models;
using Settings.Core.Services;

namespace Settings.Host.Services;

public class MockSettingsSource : SettingsSourceBase
{
    private int _counter;

    public override Task<SettingsSnapshot> GetCurrentAsync()
    {
        _counter++;
        var snapshot = new SettingsSnapshot
        {
            Name = $"Снимок {_counter}",
            Mode = _counter % 2 == 0 ? "Observe" : "Global",
            UpdatedAt = DateTimeOffset.UtcNow,
            Radio = new RadioSettings
            {
                Antenna = new AntennaSettings
                {
                    AntennaId = $"ANT-{_counter:000}",
                    Reference =
                    {
                        new SettingValue { Key = "Антенна", Value = "АРК-3" },
                        new SettingValue { Key = "МШПР", Value = "МШПР-0408" },
                        new SettingValue { Key = "Диапазон", Value = "4.00-5.00 ГГц" },
                        new SettingValue { Key = "Поляризация", Value = "V" }
                    }
                },
                Rpu = new RpuSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "Частота приемника, кГц", Value = "349995.059" },
                        new SettingValue { Key = "Аттенюатор", Value = "0" },
                        new SettingValue { Key = "Перегруз АЦП", Value = "0" }
                    }
                },
                Detector = new DetectorSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "Несущая частота, кГц", Value = "349995.148" },
                        new SettingValue { Key = "Тактовая частота, кГц", Value = "6099.998" },
                        new SettingValue { Key = "ФАПЧ", Value = "1/400" },
                        new SettingValue { Key = "Фильтр", Value = "0.4" },
                        new SettingValue { Key = "Корректор", Value = "Отключен" }
                    }
                },
                Demodulator = new DemodulatorSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "Кодер", Value = "ISU2" },
                        new SettingValue { Key = "Модуляция", Value = "ACM" },
                        new SettingValue { Key = "Пилоты", Value = "..." },
                        new SettingValue { Key = "Инверсия спектра", Value = "Вкл" },
                        new SettingValue { Key = "ОСШ", Value = "29.6" }
                    }
                },
                Decoder = new DecoderSettings
                {
                    Parameters =
                    {
                        new SettingValue { Key = "Кодер", Value = "ISU2" },
                        new SettingValue { Key = "Итераций", Value = "127" },
                        new SettingValue { Key = "Температура", Value = "46" },
                        new SettingValue { Key = "Сигнатура", Value = "Вкл" }
                    }
                }
            }
        };

        return Task.FromResult(snapshot);
    }

    // ApplyAsync uses base logic; this mock only generates snapshots.
}
