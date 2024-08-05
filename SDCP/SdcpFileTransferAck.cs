namespace Sdcp;

public enum SdcpFileTransferAck
{
  Success = 0,  // Success
  NotTransfer = 1,  // The printer is not currently transferring files.
  Checking = 2,  // The printer is already in the file verification phase.
  NotFound = 3,  // File not found.
}