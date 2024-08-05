using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sdcp;

namespace SdcpMonitor;

public partial class SettingsViewModel : ObservableRecipient
{
  private readonly ILogger<SettingsViewModel> _logger;
  private readonly Settings _settings;
  private readonly IDeviceDiscovery _deviceDiscovery;
  [ObservableProperty] private string _status = "---";
  [ObservableProperty] private string _title = "Settings";
  [ObservableProperty] private bool _minimizeToTrayOnClose;
  [ObservableProperty] private bool _connectToPrinterAtStart;
  [ObservableProperty] private string _currentPrinter = "";
  [ObservableProperty] private bool _isConnected;
  [ObservableProperty] private bool _isDiscovering;
  [ObservableProperty] private bool _showAdvancedUi;
  [ObservableProperty] private ObservableCollection<string> _printers = [];

  [ObservableProperty] private ObservableCollection<string> _themes =
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

  [ObservableProperty] private string _currentTheme = "";
  private IDeviceCommunication? _deviceCommunication;
  private Device? _device;
  private bool _settingsChanged;

  public SettingsViewModel(ILogger<SettingsViewModel> logger, Settings settings, IDeviceDiscovery deviceDiscovery)
  {
    _logger = logger;
    _settings = settings;
    _deviceDiscovery = deviceDiscovery;

    MinimizeToTrayOnClose = _settings.MinimizeToTrayOnClose;
    ConnectToPrinterAtStart = _settings.ConnectToPrinterAtStart;
    CurrentTheme = _settings.Theme == "" ? Themes[0] : _settings.Theme;
    ShowAdvancedUi = _settings.ShowAdvancedUi;
  }


  protected override void OnActivated()
  {
    base.OnActivated();

    WeakReferenceMessenger.Default.Register<DeviceDiscoveredMessage>(this, (viewModel, deviceDiscoveredMessage) =>
    {
      var self = (SettingsViewModel)viewModel;
      Application.Current.Dispatcher.InvokeAsync(() => self.OnDeviceFound(deviceDiscoveredMessage.Value));
    });
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

  private IAsyncRelayCommand? _connectCommand;
  public IAsyncRelayCommand ConnectCommand => _connectCommand ??= new AsyncRelayCommand(ConnectAsync);

  private IAsyncRelayCommand? _disconnectCommand;
  public IAsyncRelayCommand DisconnectCommand => _disconnectCommand ??= new AsyncRelayCommand(DisconnectAsync);


  private async Task OnDeviceFound(Device device)
  {
    _device = device;

    if (_settings.Printers.All(printer => printer.Id != device.Id))
    {
      _settings.Printers.Add(new Settings.Printer
      {
        Ip = device.Data.MainboardIP, Name = device.Data.Name, Id = device.Id
      });
      Printers.Add($"{device.Data.Name} ({device.Data.MainboardIP})");
    }

    if (_device == null)
    {
      Status = "Does not compute";
    }
    else
    {
      Status = $"Printer found: '{_device.Data.Name} ({_device.Data.MainboardIP})'";

      if (_settings.Printers.Count != 0)
        CurrentPrinter = $"{_settings.Printers.First().Name} ({_settings.Printers.First().Ip})";
      
      _logger.LogInformation("OnDeviceFound: {Id} {Name} {Ip}", _device.Id, _device.Data.Name,
        _device.Data.MainboardIP);
    }

    await StopDiscoveryAsync().ConfigureAwait(true);
  }


  private async Task StartDiscoveryAsync()
  {
    await StopDiscoveryAsync().ConfigureAwait(true);

    IsDiscovering = true;

    Status = "Discovery started";
    Printers.Clear();
    _settings.Printers.Clear();
    _deviceDiscovery.Start(TimeSpan.FromSeconds(5));
  }


  private async Task StopDiscoveryAsync()
  {
    IsDiscovering = false;

    if (!_deviceDiscovery.IsActive)
    {
      return;
    }

    await _deviceDiscovery.StopAsync().ConfigureAwait(true);
  }


  private async Task ConnectAsync()
  {
    if (_device == null)
    {
      return;
    }

    _deviceCommunication = App.Current.Services.GetRequiredService<IDeviceCommunication>();
    await _deviceCommunication.ConnectAsync(_device!).ConfigureAwait(true);

    IsConnected = _deviceCommunication.IsConnected;
  }


  private async Task DisconnectAsync()
  {
    IsConnected = false;

    if (_deviceCommunication == null)
    {
      return;
    }

    await _deviceCommunication.DisconnectAsync().ConfigureAwait(true);

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
