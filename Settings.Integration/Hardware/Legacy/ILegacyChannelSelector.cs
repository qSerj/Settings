namespace Settings.Integration.Hardware.Legacy;

// Adapter boundary to your legacy application state.
// Return channels exactly as your runtime represents them.
public interface ILegacyChannelSelector
{
    object GetActiveChannel();
    IReadOnlyList<object> GetAllChannels();
}
