using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace Sdcp
{
  public class DeviceDiscovery : IDeviceDiscovery, IDisposable
  {
    private const int DeviceDiscoveryPort = 3000;
    private static readonly byte[] SdcpDiscoveryPayload = "M99999"u8.ToArray();

    private readonly ILogger<DeviceDiscovery> _logger;
    private Task? _task;
    private readonly UdpClient _udpClient = new();
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly List<Device> _devices = [];
    

    public IReadOnlyList<Device> Devices => _devices;
    public bool IsActive { get; set; }
  

    public DeviceDiscovery(ILogger<DeviceDiscovery> logger)
    {
      _logger = logger;
      
      _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, DeviceDiscoveryPort));
    }


    static void RunPeriodicTask(Action action, TimeSpan interval, CancellationToken token)
    {
      Task.Run(async () =>
        {
          while (!token.IsCancellationRequested)
          {
            action();
            await Task.Delay(interval, token);
          }
        }, token);
    }

    public void Start(TimeSpan timeout)
    {
      if (IsActive)
      {
        _logger.LogError("Discovery already active");
        return;
      }

      _logger.LogInformation("Starting device discovery");

      _cancellationTokenSource = new CancellationTokenSource(timeout);
      _devices.Clear();

      try
      {
        var token = _cancellationTokenSource.Token;
        IsActive = true;
        _task = Task.Factory.StartNew(async _ =>
        {
          try
          {
            while (!token.IsCancellationRequested)
            {
              var receiveResult = await _udpClient.ReceiveAsync(token).ConfigureAwait(false);

              try
              {
                var device = System.Text.Json.JsonSerializer.Deserialize<Device>(receiveResult.Buffer)!;
                if (_devices.All(d => d.Id != device.Id))
                {
                  _devices.Add(device);
                  _logger.LogInformation("Device found: {Id} {Name} {Ip}", device.Id, device.Data.Name,
                    device.Data.MainboardIP);

                  WeakReferenceMessenger.Default.Send(new DeviceDiscoveredMessage(device));
                }
              }
              catch
              {
                Debug.WriteLine($"Response was not a Device object {receiveResult.RemoteEndPoint} : {Encoding.ASCII.GetString(receiveResult.Buffer)}");
              }

            }

            IsActive = false;
          }
          catch (Exception e)
          {
            Debug.WriteLine(e);

          }
        }, null, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);


        _udpClient.Send(SdcpDiscoveryPayload, SdcpDiscoveryPayload.Length, "255.255.255.255", DeviceDiscoveryPort);
      }
      catch (OperationCanceledException)
      {
        Debug.WriteLine("Operation canceled");
        IsActive = false;

      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
        IsActive = false;
      }
    }

    public async Task StopAsync()
    {
      if (_task != null && _cancellationTokenSource != null)
      {
        if (!_cancellationTokenSource.IsCancellationRequested)
          await _cancellationTokenSource.CancelAsync().ConfigureAwait(false);

        await _task.ConfigureAwait(false);
        _task = null;
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;

        IsActive = false;
      }
    }

    public void Dispose()
    {
      _udpClient.Dispose();
    }
  }
}
