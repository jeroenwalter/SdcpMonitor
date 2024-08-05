namespace SdcpMonitor;

public class Settings
{

  public bool MinimizeToTrayOnClose { get; set; } = true;
  public bool ConnectToPrinterAtStart { get; set; } = false;

  public class Printer
  {
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Ip { get; set; } = "";

  }

  public List<Printer> Printers { get; set; }= [];

  public bool ShowAdvancedUi { get; set; } = false;

  public string Theme { get; set; } = "";
}