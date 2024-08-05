using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Sdcp;

public class DeviceStatusMessage : ValueChangedMessage<StatusMessage>
{
  public DeviceStatusMessage(StatusMessage status)
  : base(status)
  {
    
  }
}