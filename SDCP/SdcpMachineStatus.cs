namespace Sdcp;

public enum SdcpMachineStatus
{
  Idle = 0,  // Idle
  Printing = 1,  // Executing print task
  FileTransferring = 2,  // File transfer in progress
  ExposureTesting = 3,  // Exposure test in progress
  DeviceTesting = 4  //Device self-check in progress
}