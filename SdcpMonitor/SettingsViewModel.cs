using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sdcp;
using System.Net;

namespace SdcpMonitor;

public partial class SettingsViewModel : ObservableRecipient
{
  private readonly ILogger<SettingsViewModel> _logger;
  private readonly Settings _settings;
  private readonly IDeviceDiscovery _deviceDiscovery;
  private readonly IDispatcher _dispatcher;
  [ObservableProperty] private string _status = "---";
  [ObservableProperty] private string _title = "Settings";
  [ObservableProperty] private bool _minimizeToTrayOnClose;
  [ObservableProperty] private bool _connectToPrinterAtStart;
  [ObservableProperty] private string _printer = "";
  [ObservableProperty] private bool _isConnected;
  [ObservableProperty] private bool _isDiscovering;
  [ObservableProperty] private bool _showAdvancedUi;

  [ObservableProperty]
  private ObservableCollection<string> _themes =
  [
    "<Windows default>",
    "Light.Red", "Light.Green", "Light.Blue", "Light.Purple", "Light.Orange", "Light.Lime", "Light.Emerald",
    "Light.Teal", "Light.Cyan", "Light.Cobalt", "Light.Indigo",
    "Light.Violet", "Light.Pink", "Light.Magenta", "Light.Crimson", "Light.Amber", "Light.Yellow", "Light.Brown",
    "Light.Olive", "Light.Steel", "Light.Mauve", "Light.Taupe", "Light.Sienna",
    "Dark.Red", "Dark.Green", "Dark.Blue", "Dark.Purple", "Dark.Orange", "Dark.Lime", "Dark.Emerald", "Dark.Teal",
    "Dark.Cyan", "Dark.Cobalt", "Dark.Indigo", "Dark.Violet", "Dark.Pink",
    "Dark.Magenta", "Dark.Crimson", "Dark.Amber", "Dark.Yellow", "Dark.Brown", "Dark.Olive", "Dark.Steel", "Dark.Mauve",
    "Dark.Taupe", "Dark.Sienna"
  ];

  [ObservableProperty] private string _currentTheme;
  private bool _settingsChanged;

  public SettingsViewModel(ILogger<SettingsViewModel> logger, Settings settings, IDeviceDiscovery deviceDiscovery, IDispatcher dispatcher)
  {
    _logger = logger;
    _settings = settings;
    _deviceDiscovery = deviceDiscovery;
    _dispatcher = dispatcher;

    if (_settings.Printer != null)
      _printer = _settings.Printer.ToString();

    _minimizeToTrayOnClose = _settings.MinimizeToTrayOnClose;
    _connectToPrinterAtStart = _settings.ConnectToPrinterAtStart;
    _currentTheme = _settings.Theme == "" ? Themes[0] : _settings.Theme;
    _showAdvancedUi = _settings.ShowAdvancedUi;
  }


  protected override void OnActivated()
  {
    base.OnActivated();

    WeakReferenceMessenger.Default.Register<DeviceDiscoveredMessage>(this, (viewModel, deviceDiscoveredMessage) =>
    {
      var self = (SettingsViewModel)viewModel;
      _dispatcher.InvokeAsync(() => self.OnDeviceFound(deviceDiscoveredMessage.Value));
    });
  }



  partial void OnPrinterChanged(string value)
  {
    _settingsChanged = true;
  }

  protected override void OnDeactivated()
  {
    WeakReferenceMessenger.Default.UnregisterAll(this);

    if (_settingsChanged)
      WeakReferenceMessenger.Default.Send(new SettingsChangedMessage(_settings));

    base.OnDeactivated();
  }



  private IAsyncRelayCommand? _startDiscoveryCommand;
  public IAsyncRelayCommand StartDiscoveryCommand => _startDiscoveryCommand ??= new AsyncRelayCommand(StartDiscoveryAsync);

  private IAsyncRelayCommand? _stopDiscoveryCommand;
  public IAsyncRelayCommand StopDiscoveryCommand => _stopDiscoveryCommand ??= new AsyncRelayCommand(StopDiscoveryAsync);

  
  private async Task OnDeviceFound(Device device)
  {
    Status = $"Printer found: '{device.Data.Name} ({device.Data.MainboardIP})'";
    _logger.LogInformation("OnDeviceFound: {Id} {Name} {Ip}", device.Data.MainboardID, device.Data.Name, device.Data.MainboardIP);
    
    _settings.Printer = new Printer
    {
      Ip = device.Data.MainboardIP,
      Name = device.Data.Name,
      Id = device.Data.MainboardID
    };

    Printer = _settings.Printer.ToString();
    _settingsChanged = true;
    
    await StopDiscoveryAsync().ConfigureAwait(true);
  }


  private async Task StartDiscoveryAsync()
  {
    await StopDiscoveryAsync().ConfigureAwait(true);

    IsDiscovering = true;

    Status = "Discovery started";
    _deviceDiscovery.Start(TimeSpan.FromSeconds(5));
  }


  private async Task StopDiscoveryAsync()
  {
    IsDiscovering = false;
    Status = "Discovery stopped";

    if (!_deviceDiscovery.IsActive)
    {
      return;
    }

    await _deviceDiscovery.StopAsync().ConfigureAwait(true);
  }
  

  partial void OnMinimizeToTrayOnCloseChanged(bool value)
  {
    _settings.MinimizeToTrayOnClose = value;
    _settingsChanged = true;
  }


  partial void OnConnectToPrinterAtStartChanged(bool value)
  {
    _settings.ConnectToPrinterAtStart = value;
    _settingsChanged = true;
  }


  partial void OnShowAdvancedUiChanged(bool value)
  {
    _settings.ShowAdvancedUi = value;
    _settingsChanged = true;
  }


  partial void OnCurrentThemeChanged(string value)
  {
    _settings.Theme = value == Themes[0] ? "" : value;
    _settingsChanged = true;
    App.Current.ApplyTheme();
  }
}
