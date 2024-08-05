namespace Sdcp;

public interface IDeviceDiscovery
{
  IReadOnlyList<Device> Devices { get; }
  bool IsActive { get; }
  void Start(TimeSpan timeout);
  Task StopAsync();
}