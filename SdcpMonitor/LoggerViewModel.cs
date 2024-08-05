using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace SdcpMonitor
{
  public sealed partial class LoggerViewModel : ObservableRecipient
  {
    private readonly ILogger<LoggerViewModel> _logger;

    public LoggerViewModel(ILogger<LoggerViewModel> logger)
    {
      _logger = logger;
    }
  }
}
