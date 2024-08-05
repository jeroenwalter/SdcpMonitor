﻿using Sdcp;

namespace SdcpMonitor;

public interface INavigationService
{
  void ShowAboutView();
  void ShowMainView();
  void ExitProgram();
  void ShowCameraView();
  void ShowLoggerView();
  void ShowSettingsView();
}