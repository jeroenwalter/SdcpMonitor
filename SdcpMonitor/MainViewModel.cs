using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sdcp;

namespace SdcpMonitor;

public sealed partial class MainViewModel : ObservableRecipient
{
  private readonly ILogger<MainViewModel> _logger;

  [ObservableProperty] private string _title = "Sdcp Monitor";
  [ObservableProperty] private string _status = "---";
  [ObservableProperty] private string _layers = "---";
  [ObservableProperty] private string _duration = "---";
  [ObservableProperty] private string _progress = "---";
  [ObservableProperty] private string _startTime = "---";
  [ObservableProperty] private string _stopTime = "---";
  [ObservableProperty] private string _file = "---";
  [ObservableProperty] private string _printerName = "---";
  [ObservableProperty] private string _trayToolTip = "Sdcp Monitor";
  [ObservableProperty] private string _currentPrinter = "";
  [ObservableProperty] private bool _isConnected;
  [ObservableProperty] private bool _isDiscovering;
  [ObservableProperty] private bool _showAdvancedUi;
  [ObservableProperty] private string _temperatures = "";
  [ObservableProperty] private long _releaseFilmCount;
  [ObservableProperty] private TimeSpan _screenExposureTime;
  [ObservableProperty] private string _usage = "";
  [ObservableProperty] private string _calculatedStopTime = "";

  [ObservableProperty] private ObservableCollection<string> _printers = [];

  private readonly INavigationService _navigationService;
  private readonly Settings _settings;
  private IDeviceCommunication? _deviceCommunication;
  private Device? _device;


  public MainViewModel(ILogger<MainViewModel> logger,
    INavigationService navigationService,
    Settings settings)
  {
    _logger = logger;
    _navigationService = navigationService;
    _settings = settings;
  }


  protected override async void OnActivated()
  {
    base.OnActivated();

    WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, (viewModel, settingsChangedMessage) =>
    {
      var self = (MainViewModel)viewModel;
      Application.Current.Dispatcher.InvokeAsync(() => self.SyncWithSettingsAsync(settingsChangedMessage.Value));
    });

    WeakReferenceMessenger.Default.Register<DeviceStatusMessage>(this, (viewModel, deviceStatusMessage) =>
    {
      var self = (MainViewModel)viewModel;
      Application.Current.Dispatcher.InvokeAsync(() => self.UpdateStatus(deviceStatusMessage.Value));
    });

    await SyncWithSettingsAsync(_settings).ConfigureAwait(true);
  }


  private async Task SyncWithSettingsAsync(Settings settings)
  {
    ShowAdvancedUi = _settings.ShowAdvancedUi;

    _settings.Printers.ForEach(printer => Printers.Add($"{printer.Name} ({printer.Ip})"));

    if (_settings.Printers.Count != 0)
      CurrentPrinter = $"{_settings.Printers.First().Name} ({_settings.Printers.First().Ip})";

    if (!_settings.ConnectToPrinterAtStart || _settings.Printers.Count == 0) return;

    Settings.Printer printer = _settings.Printers.First();
    if (IsConnected && _device?.Id != printer.Id)
      await DisconnectAsync().ConfigureAwait(true);

    _device = new Device()
    {
      Data = { MainboardID = printer.Id, MainboardIP = printer.Ip, MachineName = printer.Name, Name = printer.Name }
    };

    await ConnectAsync().ConfigureAwait(true);
  }


  protected override void OnDeactivated()
  {
    WeakReferenceMessenger.Default.UnregisterAll(this);

    base.OnDeactivated();
  }


  private IRelayCommand? _showLoggerCommand;
  public IRelayCommand ShowLoggerCommand => _showLoggerCommand ??= new RelayCommand(_navigationService.ShowLoggerView);

  private IRelayCommand? _showSettingsCommand;

  public IRelayCommand ShowSettingsCommand =>
    _showSettingsCommand ??= new RelayCommand(_navigationService.ShowSettingsView);


  private IAsyncRelayCommand? _refreshCommand;

  public IAsyncRelayCommand RefreshCommand =>
    _refreshCommand ??= new AsyncRelayCommand(RefreshStatusAndAttributesAsync);


  private async Task RefreshStatusAndAttributesAsync()
  {
    if (_deviceCommunication == null)
      return;

    await _deviceCommunication.GetAttributesAsync().ConfigureAwait(true);
    await _deviceCommunication.GetStatusAsync().ConfigureAwait(true);
  }


  private IRelayCommand? _showAboutCommand;
  public IRelayCommand ShowAboutCommand => _showAboutCommand ??= new RelayCommand(_navigationService.ShowAboutView);

  private IRelayCommand? _showMainWindowCommand;

  public IRelayCommand ShowMainWindowCommand =>
    _showMainWindowCommand ??= new RelayCommand(_navigationService.ShowMainView);

  private IRelayCommand? _exitProgramCommand;
  public IRelayCommand ExitProgramCommand => _exitProgramCommand ??= new RelayCommand(_navigationService.ExitProgram);

  private IRelayCommand? _showCameraCommand;
  public IRelayCommand ShowCameraCommand =>
    _showCameraCommand ??= new RelayCommand(() => _navigationService.ShowCameraView(),
      () => _deviceCommunication?.IsConnected == true);


  private void UpdateStatus(StatusMessage status)
  {
    PrinterName = _device?.Data.Name ?? "---";
    Status = string.Join(",", status.Status.CurrentStatus);
    Progress = status.Status.PrintInfo.TotalLayer != 0
      ? $"{100.0 * status.Status.PrintInfo.CurrentLayer / status.Status.PrintInfo.TotalLayer:F2} %"
      : "---";
    
    Layers = $"current {status.Status.PrintInfo.CurrentLayer} | total {status.Status.PrintInfo.TotalLayer}";
    TimeSpan currentTicks = TimeSpan.FromMilliseconds(status.Status.PrintInfo.CurrentTicks);
    TimeSpan totalTicks = TimeSpan.FromMilliseconds(status.Status.PrintInfo.TotalTicks);
    DateTime now = DateTime.Now;
    DateTime start = now - currentTicks;
    DateTime stop = start + totalTicks;
    
    // TODO: take into account the time needed for the burn-in layers and the transition layers
    // calculate layer time from delta between 2 layers, determine that burn-in and transition layers are done
    double msPerLayer = status.Status.PrintInfo.CurrentLayer > 0 ? (double)status.Status.PrintInfo.CurrentTicks / (double)status.Status.PrintInfo.CurrentLayer : 0;
    TimeSpan eta = TimeSpan.FromMilliseconds(status.Status.PrintInfo.TotalLayer * msPerLayer);
    DateTime calculatedStop = start + eta;
    CalculatedStopTime = $@"{calculatedStop:HH:mm:ss} | Duration: {eta:dd\.hh\:mm\:ss} | Per Layer {1000.0 * msPerLayer:F2} s";

    StartTime = $"{start:HH:mm:ss}";
    StopTime = $"{stop:HH:mm:ss}";
    Duration = $@"{currentTicks:dd\.hh\:mm\:ss} / {totalTicks:dd\.hh\:mm\:ss}";
    
    File = $"{status.Status.PrintInfo.Filename}";
    ReleaseFilmCount = status.Status.ReleaseFilm;
    ScreenExposureTime = TimeSpan.FromSeconds(status.Status.PrintScreen);
    Temperatures = $"UV Led {status.Status.TempOfUVLED:F1} \u2103";
    //$"Box: {status.Status.TempOfBox:F1} | UV Led {status.Status.TempOfUVLED:F1} | Target Box {status.Status.TempTargetBox:F1}";

    Usage = $"Film {ReleaseFilmCount/60000.0 * 100.0:F1}% ({ReleaseFilmCount}/60000) | Screen {ScreenExposureTime.Days * 24 + ScreenExposureTime.Hours} hours";

    TrayToolTip = $"{Status}\n{File}\n{Progress} ({Layers})\nETA: {StopTime}\nTemperature: {status.Status.TempOfUVLED:F1} \u2103\nFilm {ReleaseFilmCount / 60000.0 * 100.0:F1}% ({ReleaseFilmCount}/60000)";
    WeakReferenceMessenger.Default.Send(new CameraOverlayTextChangedMessage(TrayToolTip));
  }


  private async Task ConnectAsync()
  {
    if (_device == null) 
      return;

    _logger.LogInformation("Connecting to {}", _device.Data.MainboardIP);

    _deviceCommunication = App.Current.Services.GetRequiredService<IDeviceCommunication>();
    await _deviceCommunication.ConnectAsync(_device!).ConfigureAwait(true);
    ShowCameraCommand.NotifyCanExecuteChanged();
    IsConnected = _deviceCommunication.IsConnected;

    _logger.LogInformation(IsConnected ? "Connected to {}" : "Failed to connect to {}", _device.Data.MainboardIP);
  }


  private async Task DisconnectAsync()
  {
    IsConnected = false;

    if (_deviceCommunication == null)
      return;

    await _deviceCommunication.DisconnectAsync().ConfigureAwait(true);

    _logger.LogInformation("Disconnected from {}", _device?.Data.MainboardIP);

    ShowCameraCommand.NotifyCanExecuteChanged();
  }
}