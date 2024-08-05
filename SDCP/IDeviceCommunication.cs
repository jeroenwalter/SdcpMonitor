namespace Sdcp;

public interface IDeviceCommunication
{
  string VideoUrl { get; }
  bool IsConnected { get; }
  Task ConnectAsync(Device device);
  Task DisconnectAsync();
  Task GetStatusAsync();
  Task GetAttributesAsync();
  Task EnableVideoStreamAsync(bool enable);
}