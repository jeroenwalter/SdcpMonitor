using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Sdcp;

public class DeviceDiscoveredMessage : ValueChangedMessage<Device>
{
  public DeviceDiscoveredMessage(Device device)
    : base(device)
  {

  }
}