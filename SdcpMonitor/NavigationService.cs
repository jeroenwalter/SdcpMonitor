using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Sdcp;

namespace SdcpMonitor
{
  internal class NavigationService : INavigationService
  {
    private LoggerView? _loggerView;
    private MainView MainView => (MainView)App.Current.MainWindow!;


    public void ShowAboutView()
    {
      var window = App.Current.Services.GetRequiredService<AboutView>();
      window.Owner = App.Current.MainWindow;
      window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      window.ShowDialog();
    }


    public void ShowMainView()
    {
      MainView.RestoreFromTray();
    }
    

    public void ExitProgram()
    {
      App.Current.IsClosing = true;

      MainView.Close();
    }


    public void ShowCameraView()
    {
      var window = App.Current.Services.GetRequiredService<CameraView>();
      window.Owner = App.Current.MainWindow;
      window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      window.Show();
    }

    public void ShowLoggerView()
    {
      if (_loggerView == null)
      {
        _loggerView = App.Current.Services.GetRequiredService<LoggerView>();
        _loggerView.Owner = App.Current.MainWindow;
      }

      _loggerView.Show();
    }

    public void ShowSettingsView()
    {
      var window = App.Current.Services.GetRequiredService<SettingsView>();
      window.Owner = App.Current.MainWindow;
      window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      window.ShowDialog();
    }
  }
}
