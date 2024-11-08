namespace SdcpMonitor;

public class Printer
{
  public string Id { get; set; } = "";
  public string Name { get; set; } = "";
  public string Ip { get; set; } = "";
  
  public override string ToString() => $"{Name} ({Ip})";
}