# Architecture Overview

Этот документ описывает, как устроена подсистема настроек и где расширять.

## Слои
`Settings.Core`
- Модели: `SettingsSnapshot`, `RadioSettings`, `SettingsBlock` и его узлы,
  `SettingsSection`, `SettingValue`
- Репозиторий: `ISettingsRepository` + `JsonSettingsRepository`
- Источник текущих настроек: `ISettingsSource`
- Применение: `ISettingsApplier` + `ISettingsApplyStrategy`

`Settings.Controls`
- UI для списка снимков и управления
- ViewModel: `SettingsViewUserControlViewModel`
- Лог шагов применения: `ApplyStepViewModel`

`Settings.Host`
- DI регистрация реализаций
- Заглушка источника: `MockSettingsSource`
- Стратегии применения по режимам

## Данные
Снимок:
- `Name`, `Mode`, `CreatedAt`, `UpdatedAt`
- `Radio`: цепочка узлов `Antenna`, `Rpu`, `Detector`, `Demodulator`, `Decoder`

Узел тракта (`SettingsBlock`):
- `IsPresent`, `IsRelevant` — флаги наличия и актуальности
- `Parameters` — плоский набор параметров
- `Sections` — вложенные секции для сложных случаев

Антенна (`AntennaSettings`) дополнительно:
- `AntennaId` — ссылка на отдельное хранилище антенн
- `Tuning` — минимальный набор чисел для настройки
- `Reference` — справочные параметры

Сводка цепочки:
- формируется в `SettingsSnapshotViewModel.SectionsSummary` как сводка по узлам цепочки

## Хранение
`JsonSettingsRepository` сохраняет данные в:
- `%APPDATA%/Settings.Host/settings_snapshots.json`
- при отсутствии копирует шаблон из `assets/settings_snapshots.json`

## Применение настроек
Применение выполняется стратегией по `Mode`:
1. `SettingsApplier` выбирает `ISettingsApplyStrategy` по `snapshot.Mode`
2. Стратегия выполняет шаги последовательно
3. `IApplyReporter` пишет в UI список шагов и статусы

Если шаг падает:
- возвращается `ApplyResult.Failed`
- UI показывает ошибку и последний шаг

## Расширение
Новые режимы:
1. Создать стратегию `ISettingsApplyStrategy` с нужным `Mode`
2. Добавить регистрацию в DI

Детализация шагов:
- разбить `ApplyAsync` стратегии на подметоды
- писать в reporter по каждому подэтапу

Интеграция с аппаратурой:
- заменить `MockSettingsSource` на реальный адаптер
- `ISettingsSource.ApplyAsync` должна быть идемпотентной по возможности

## Пример потоков по режимам

`Global`
1. Подготовка
2. Применение настроек (узлы: Antenna, Rpu, Detector, Demodulator, Decoder)
3. Завершение

`Observe`
1. Подготовка
2. Применение мониторинга (узлы: Rpu, Detector)
