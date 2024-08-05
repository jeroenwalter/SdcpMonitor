using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SdcpMonitor;

public class SettingsChangedMessage : ValueChangedMessage<Settings>
{
  public SettingsChangedMessage(Settings settings)
    : base(settings)
  {

  }
}