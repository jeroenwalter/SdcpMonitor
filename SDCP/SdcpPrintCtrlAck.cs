namespace Sdcp;

public enum SdcpPrintCtrlAck
{
  Ok = 0,  // OK
  Busy = 1,  // Busy
  NotFound = 2,  // File Not Found
  Md5Failed = 3,  // MD5 Verification Failed
  FileIoFailed = 4,  // File Read Failed
  InvalidResolution = 5, // Resolution Mismatch
  UnknownFormat = 6,  // Unrecognized File Format
  UnknownModel = 7  // Machine Model Mismatch
}