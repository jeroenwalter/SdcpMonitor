using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SdcpMonitor;

internal class SettingsStorage 
{
  private readonly ILogger<SettingsStorage> _logger;
  private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };


  public SettingsStorage(ILogger<SettingsStorage> logger)
  {
    _logger = logger;
  }


  public void Save<TSettings>(TSettings settings, string path)
    where TSettings : class
  {
    try
    {
      var json = JsonSerializer.Serialize(settings, options: _jsonSerializerOptions);
      Directory.CreateDirectory(Path.GetDirectoryName(path)!);
      File.WriteAllText(path, json);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Saving {Path} failed", path);
    }
  }


  public TSettings Load<TSettings>(string path, bool backupCorrupted)
    where TSettings : class, new()
  {
    var defaultSettings = new TSettings();

    try
    {
      if (File.Exists(path))
      {
        try
        {
          var json = File.ReadAllText(path);
          var settings = JsonSerializer.Deserialize<TSettings>(json) ?? throw new NullReferenceException();
          return settings;
          
        }
        catch (Exception exception)
        {
          _logger.LogError(exception, "Failed to read settings file {Path}, content is invalid", path);
          var id = DateTime.Now.ToString("yyyyMMdd-hhmmss");
          if (backupCorrupted)
          {
            File.Copy(path, $"{path}.corrupt-{id}", true);
            File.Delete(path);
          }
        }
      }
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Loading {Path} failed", path);
    }
    
    Save(defaultSettings, path);
    return defaultSettings;
  }
}