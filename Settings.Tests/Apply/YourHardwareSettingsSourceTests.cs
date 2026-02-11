using Settings.Core.Models;
using Settings.Integration.Hardware;
using Settings.Integration.Services;
using Xunit;

namespace Settings.Tests.Apply;

public class YourHardwareSettingsSourceTests
{
    [Fact]
    public async Task GetCurrentAsync_UsesCollectorForCurrentMode()
    {
        var source = new YourHardwareSettingsSource(
            new StubContextProvider("Observe"),
            new IModeSnapshotCollector[]
            {
                new StubCollector("Global", "global-name"),
                new StubCollector("Observe", "observe-name")
            });

        var snapshot = await source.GetCurrentAsync();

        Assert.Equal("Observe", snapshot.Mode);
        Assert.Equal("observe-name", snapshot.Name);
    }

    [Fact]
    public async Task GetCurrentAsync_WhenCollectorMissing_Throws()
    {
        var source = new YourHardwareSettingsSource(
            new StubContextProvider("Unknown"),
            new IModeSnapshotCollector[]
            {
                new StubCollector("Global", "global-name")
            });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => source.GetCurrentAsync());

        Assert.Contains("Unknown", ex.Message);
    }

    private sealed class StubContextProvider : IRuntimeContextProvider
    {
        private readonly string _mode;

        public StubContextProvider(string mode)
        {
            _mode = mode;
        }

        public RuntimeContext GetCurrent() =>
            new()
            {
                Mode = _mode,
                Root = null
            };
    }

    private sealed class StubCollector : IModeSnapshotCollector
    {
        public string Mode { get; }
        private readonly string _name;

        public StubCollector(string mode, string name)
        {
            Mode = mode;
            _name = name;
        }

        public SettingsSnapshot Collect(RuntimeContext context) =>
            new()
            {
                Name = _name,
                Mode = context.Mode,
                Radio = new RadioSettings()
            };
    }
}
