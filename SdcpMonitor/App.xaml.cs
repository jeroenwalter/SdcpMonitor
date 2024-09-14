using System.IO;
using System.Windows;
using ControlzEx.Theming;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Sdcp;
using LogLevel = NLog.LogLevel;

namespace SdcpMonitor
{
  public sealed partial class App
  {
    public const string ApplicationName = "SdcpMonitor";
    public const string FileVersion = "1.0.0.0";
    public const string ApplicationVersion = "1.0.0-alpha";

    private readonly IHost _host;
    private readonly string _settingsPath;
    private Settings? _settings;
    private readonly string _programDataPath;
    private Logger? _logger;

    public static new App Current => (App)Application.Current;
    public IServiceProvider Services => _host.Services;

    public bool IsClosing { get; set; }

    public App()
    {
      _programDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        ApplicationName);

      _settingsPath = Path.Combine(_programDataPath, "settings.json");
      SetupLogger();

      _host = new HostBuilder()
        .ConfigureAppConfiguration((context, configurationBuilder) =>
        {
          configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
          configurationBuilder.AddJsonFile("appsettings.json", optional: false);
        })
        .ConfigureServices((context, services) =>
        {
          services.Configure<Settings>(context.Configuration);
          ConfigureServices(services);
        })
        .ConfigureLogging(logging =>
        {
          logging.ClearProviders();
          logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
          logging.AddNLog();
        })
        .Build();
    }

    private void SetupLogger()
    {
      LogManager.Setup().LoadConfigurationFromFile("nlog.config");
      LogManager.Configuration.Variables["basedir"] = _programDataPath;
      _logger = LogManager.GetCurrentClassLogger();
      _logger.Log(LogLevel.Info, "------------------------------------------------------");
      _logger.Log(LogLevel.Info, $"{ApplicationName} started");
    }

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
      await _host.StartAsync();

      _settings = Services.GetRequiredService<SettingsStorage>().Load<Settings>(_settingsPath, true);
      
      ApplyTheme();

      Services.GetRequiredService<MainView>().Show();
    }

    public void ApplyTheme()
    {
      try
      {
        if (string.IsNullOrWhiteSpace(_settings!.Theme))
        {
          ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
          ThemeManager.Current.SyncTheme();
        }
        else
          ThemeManager.Current.ChangeTheme(this, _settings!.Theme);
      }
      catch
      {
        _logger!.Log(LogLevel.Error, "Invalid UI Theme '{Theme}', reverting back to Windows default", _settings!.Theme);
        _settings.Theme = "";
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
        ThemeManager.Current.SyncTheme();
      }
    }

    private async void Application_Exit(object sender, ExitEventArgs e)
    {
      using (_host)
      {
        if (_settings != null)
        {
          var settingsStorage = Services.GetRequiredService<SettingsStorage>();
          settingsStorage.Save(_settings, _settingsPath);
        }

        await _host.StopAsync(TimeSpan.FromSeconds(5));

        _logger?.Log(LogLevel.Info, $"{ApplicationName} stopped");
        _logger?.Log(LogLevel.Info, "------------------------------------------------------");
        LogManager.Shutdown();
      }
    }


    private void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<SettingsStorage>();
      services.AddSingleton(_ => _settings!);

      services.AddSingleton<IDeviceDiscovery, DeviceDiscovery>();
      services.AddTransient<IDeviceCommunication, DeviceCommunication>();

      services.AddSingleton<INavigationService, NavigationService>();
      services.AddTransient<MainView>();
      services.AddTransient<MainViewModel>();
      services.AddTransient<LoggerView>();
      services.AddTransient<LoggerViewModel>();
      services.AddTransient<AboutView>();
      services.AddTransient<SettingsView>();
      services.AddTransient<SettingsViewModel>();
    }
  }
}