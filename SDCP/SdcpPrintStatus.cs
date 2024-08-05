namespace Sdcp;

public enum SdcpPrintStatus
{
  Idle = 0,  // Idle
  Homing = 1,  // Resetting
  Dropping = 2,  // Descending
  Exposing = 3,  // Exposing
  Lifting = 4,  // Lifting
  Pausing = 5,  // Executing Pause Action
  Paused = 6,  // Suspended
  Stopping = 7,  // Executing Stop Action
  Stopped = 8,  // Stopped
  Complete = 9,  // Print Completed
  FileChecking = 10 // File Checking in Progress
}