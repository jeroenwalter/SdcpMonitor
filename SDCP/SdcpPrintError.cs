namespace Sdcp;

public enum SdcpPrintError
{
  None = 0,  // Normal
  Check = 1,  // File MD5 Check Failed
  FileIo = 2,  // File Read Failed
  InvalidResolution = 3,  // Resolution Mismatch
  UnknownFormat = 4,  // Format Mismatch
  UnknownModel = 5  // Machine Model Mismatch
}