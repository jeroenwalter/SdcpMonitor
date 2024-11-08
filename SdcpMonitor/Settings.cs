namespace SdcpMonitor;

public class Settings
{
  public bool MinimizeToTrayOnClose { get; set; } = true;
  public bool ConnectToPrinterAtStart { get; set; } = false;
  
  public Printer? Printer { get; set; }
  
  public bool ShowAdvancedUi { get; set; } = false;

  public string Theme { get; set; } = "";
}