namespace SdcpMonitor;

internal interface ISettingsStorage
{
  void Save<TSettings>(TSettings settings, string path)
    where TSettings : class;


  TSettings Load<TSettings>(string path, bool backupCorrupted)
    where TSettings : class, new();
}