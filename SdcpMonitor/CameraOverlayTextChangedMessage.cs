using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SdcpMonitor;

public class CameraOverlayTextChangedMessage : ValueChangedMessage<string>
{
  public CameraOverlayTextChangedMessage(string overlayText)
    : base(overlayText)
  {

  }
}