// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
namespace Sdcp;

public enum SdcpErrorStatusReason
{
  Ok = 0,  // Normal
  TempError = 1,  // Over-temperature
  CalibrateFailed = 2,  // Strain Gauge Calibration Failed
  ResinLack = 3,  // Resin Level Low Detected
  ResinOver = 4,  // The volume of resin required by the model exceeds the maximum capacity of the resin vat
  ProbeFail = 5,  // No Resin Detected
  ForeignBody = 6,  // Foreign Object Detected
  LevelFailed = 7,  // Auto-leveling Failed
  ReleaseFailed = 8,  // Model Detachment Detected
  SgOffline = 9,  // Strain Gauge Not Connected
  LcdDetFailed = 10,  // LCD Screen Connection Abnormal
  ReleaseOvercount = 11,  // The cumulative release film usage has reached the maximum value
  UdiskRemove = 12,  // USB drive detected as removed, printing has been stopped
  HomeFailedX = 13,  // Detection of X-axis motor anomaly, printing has been stopped
  HomeFailedZ = 14,  // Detection of Z-axis motor anomaly, printing has been stopped
  ResinAbnormalHigh = 15,  // The resin level has been detected to exceed the maximum value, and printing has been stopped
  ResinAbnormalLow = 16,  // Resin level detected as too low, printing has been stopped
  HomeFailed = 17,  // Home position calibration failed, please check if the motor or limit switch is functioning properly
  PlatFailed = 18,  // A model is detected on the platform; please clean it and then restart printing
  Error = 19,  // Printing Exception
  MoveAbnormal = 20, // Motor Movement Abnormality
  AicModelNone = 21,  // No model detected, please troubleshoot
  AicModelWarp = 22,  // Warping of the model detected, please investigate
  HomeFailedY = 23,  // Deprecated
  FileError = 24,  // Error File
  CameraError = 25,  // Camera Error. Please check if the camera is properly connected, or you can also disable this feature to continue printing
  NetworkError = 26,  // Network Connection Error. Please check if your network connection is stable, or you can also disable this feature to continue printing
  ServerConnectFailed = 27, // Server Connection Failed. Please contact our customer support, or you can also disable this feature to continue printing
  DisconnectApp = 28,  // This printer is not bound to an app. To perform time-lapse photography, please first enable the remote control feature, or you can also disable this feature to continue printing
  CheckAutoResinFeeder = 29,  // lease check the installation of the "automatic material extraction / feeding machine"
  ContainerResinLow = 30,  // The resin in the container is running low. Add more resin to automatically close this notification, or click "Stop Auto Feeding" to continue printing
  BottleDisconnect = 31,  // Please ensure that the automatic material extraction/feeding machine is correctly installed and the data cable is connected
  FeedTimeout = 32,  // Automatic material extraction timeout, please check if the resin tube is blocked
  TankTempSensorOffline = 33,  // Resin vat temperature sensor not connected
  TankTempSensorError = 34  // Resin vat temperature sensor indicates an over-temperature condition
}