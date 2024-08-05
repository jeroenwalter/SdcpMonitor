// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
namespace Sdcp;

public class StatusMessage
{
  public string MainboardID { get; set; } = "";
  public ulong TimeStamp { get; set; } = 0;

  public string Topic { get; set; } = "";

  public StatusPayload Status { get; set; } = new();

  public class StatusPayload
  {
    public SdcpMachineStatus[] CurrentStatus { get; set; } = [];
    public SdcpMachineStatus PreviousStatus { get; set; }

    public double PrintScreen { get; set; } // Total Exposure Screen Usage Time(s)
    public long ReleaseFilm { get; set; }  // Total Release Film Usage Count
    public double TempOfUVLED { get; set; }  // Current UVLED Temperature（℃）
    public long TimeLapseStatus { get; set; }  // Time-lapse Photography Switch Status. 0: Off, 1: On
    public long TempOfBox { get; set; }  // Current Enclosure Temperature（℃）
    public long TempTargetBox { get; set; }  // Target Enclosure Temperature（℃）
    public PrintInfo PrintInfo { get; set; } = new();
  }

  public class PrintInfo
  {
    public SdcpPrintStatus Status { get; set; }  // Printing Sub-status
    public long CurrentLayer { get; set; }  // Current Printing Layer
    public long TotalLayer { get; set; }  // Total Number of Print Layers
    public long CurrentTicks { get; set; }  // Current Print Time (ms)
    public long TotalTicks { get; set; }  // Estimated Total Print Time(ms)
    public string Filename { get; set; } = "";  // Print File Name
    public long ErrorNumber { get; set; } // Refer to the following text
    public string TaskId { get; set; } = "";  // Current Task ID
  }

}