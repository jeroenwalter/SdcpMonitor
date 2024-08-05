using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;

namespace SdcpMonitor;

public partial class MainView : MetroWindow
{
  private readonly Settings _settings;
  private readonly MainViewModel _viewModel;

  public MainView(MainViewModel viewModel, Settings settings)
  {
    _viewModel = viewModel;
    _settings = settings;
    InitializeComponent();

    StateChanged += OnStateChanged;
    DataContext = viewModel;
  }

  private void OnStateChanged(object? sender, EventArgs e)
  {
    if (WindowState == WindowState.Minimized && _settings.MinimizeToTrayOnClose)
      MinimizeToTray();
  }

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);

    _viewModel.IsActive = true;
  }

  protected override void OnClosed(EventArgs e)
  {
    _viewModel.IsActive = false;

    base.OnClosed(e);
  }

  protected override void OnClosing(CancelEventArgs e)
  {
    if (_settings.MinimizeToTrayOnClose && !App.Current.IsClosing)
    {
      MinimizeToTray();
      e.Cancel = true;
    }

    base.OnClosing(e);
  }

  private void MinimizeToTray()
  {
    WindowState = WindowState.Minimized;
    ShowInTaskbar = false;
  }

  public void RestoreFromTray()
  {
    ShowInTaskbar = true;
    WindowState = WindowState.Normal;
  }
}