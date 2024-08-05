// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
namespace Sdcp;


/*
 *"Id": "xxx",  // Machine brand identifier, 32-bit UUID
   "Data": {
       "Name": "PrinterName",  // Machine Name
       "MachineName": "MachineModel",  // Machine Model
       "BrandName": "CBD",  // Brand Name
       "MainboardIP": "192.168.1.2",  // Motherboard IP Address
       "MainboardID": "000000000001d354",  // Motherboard ID(16bit)
       "ProtocolVersion": "V3.0.0",  // Protocol Version
       "FirmwareVersion": "V1.0.0"  // Firmware Version
   }
 */

public class Device
{
  public string Id { get; set; } = "";

  public DeviceData Data { get; set; } = new();

  public class DeviceData
  {
    public string Name { get; set; } = "";
    public string MachineName { get; set; } = "";
    public string BrandName { get; set; } = "";
    public string MainboardIP { get; set; } = "";
    public string MainboardID { get; set; } = "";
    public string ProtocolVersion { get; set; } = "";
    public string FirmwareVersion { get; set; } = "";
  }
}