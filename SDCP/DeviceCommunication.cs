using System;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace Sdcp;

public class DeviceCommunication : IDeviceCommunication
{
  private readonly ILogger<DeviceCommunication> _logger;
  private readonly TimeProvider _timeProvider;
  private readonly ITimer _timer;
  private const long StatusCommand = 0;
  private const long AttributesCommand = 1;
  private const long EnableDisableVideoStreamCommand = 386;
  private ClientWebSocket? _webSocket;
  private Task? _messageHandlerTask;
  private CancellationTokenSource? _cancellationTokenSource;
  private Device? _device;
  private ulong _nextRequestId = 1;
  private ulong _requestTimeStamp = 0;
  //private long _lastStatusUpdateTimestamp;
  public string VideoUrl { get; private set; } = "";


  public DeviceCommunication(ILogger<DeviceCommunication> logger, TimeProvider timeProvider)
  {
    _logger = logger;
    _timeProvider = timeProvider;
    _timer = _timeProvider.CreateTimer(RefreshStatusTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
  }


  private void RefreshStatusTimerCallback(object? state)
  {
    EventHandlers.TryAsync<EventArgs>(async (s, e) =>
      {
        await RefreshStatus().ConfigureAwait(false);
      },
    ex => Console.WriteLine($"[TryAsync Error Callback] Our exception handler caught: {ex}")).Invoke(null, EventArgs.Empty);
  }


  public async Task RefreshStatus()
  {
    try
    {
      await GetStatusAsync().ConfigureAwait(false);
      await GetAttributesAsync().ConfigureAwait(false);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      //throw;
    }
  }


  public async Task ConnectAsync(Device device)
  {
    _device = device ?? throw new ArgumentNullException(nameof(device));

    if (_webSocket != null)
    {
      _logger.LogError("WebSocket already open");
      throw new InvalidOperationException("WebSocket already open");
    }

    var remote = IPEndPoint.Parse($"{_device.Data.MainboardIP}:3030");
    _logger.LogInformation("Opening web socket connection to {Ip}", remote);

    _cancellationTokenSource = new CancellationTokenSource();

    _webSocket = new ClientWebSocket();
    await _webSocket.ConnectAsync(new Uri($"ws://{remote}/websocket"), _cancellationTokenSource.Token).ConfigureAwait(false);

    if (_webSocket.State == WebSocketState.Open)
    {
      _logger.LogInformation("Connected to {Name} on {Ip}", _device.Data.MachineName, remote);
      _messageHandlerTask = Task.Run(() => ReceiveMessages(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
      
      await RefreshStatus().ConfigureAwait(false);
    }
    else
    {
      _logger.LogError("Failed to connect to {Ip}", remote);
    }
  }

  public async Task DisconnectAsync()
  {
    if (_webSocket == null || _cancellationTokenSource == null)
      throw new InvalidOperationException("WebSocket already closed");

    await _cancellationTokenSource.CancelAsync().ConfigureAwait(false);
    try
    {
      await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      //throw;
    }

    _webSocket.Dispose();
    _webSocket = null;
    _cancellationTokenSource.Dispose();
    _cancellationTokenSource = null;

    if (_messageHandlerTask != null)
    {
      await _messageHandlerTask.ConfigureAwait(false);
      _messageHandlerTask = null;
    }
  }

  public bool IsConnected => _webSocket?.State == WebSocketState.Open;


  public async Task GetStatusAsync()
  {
    if (!IsConnected || _device == null || _cancellationTokenSource is { IsCancellationRequested: true })
      return;

    _requestTimeStamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    var message = new SdcpRequest
    {
      Id = _device.Id,
      Topic = $"sdcp/request/{_device.Data.MainboardID}",
      Data =
      {
        Cmd = StatusCommand,
        RequestID = $"{_nextRequestId:X016}",
        From = (int)SdcpFrom.Pc,
        MainboardID = _device.Data.MainboardID,
        TimeStamp = _requestTimeStamp
      }
    };

    _nextRequestId++;

    await SendString(_webSocket!, JsonSerializer.Serialize(message), _cancellationTokenSource!.Token).ConfigureAwait(false);
  }


  public async Task GetAttributesAsync()
  {
    if (!IsConnected || _device == null || _cancellationTokenSource is { IsCancellationRequested: true })
      return;

    _requestTimeStamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    var message = new SdcpRequest
    {
      Id = _device.Id,
      Topic = $"sdcp/request/{_device.Data.MainboardID}",
      Data =
      {
        Cmd = AttributesCommand,
        RequestID = $"{_nextRequestId:X016}",
        From = (int)SdcpFrom.Pc,
        MainboardID = _device.Data.MainboardID,
        TimeStamp = _requestTimeStamp
      }
    };

    _nextRequestId++;

    await SendString(_webSocket!, JsonSerializer.Serialize(message), _cancellationTokenSource!.Token).ConfigureAwait(false);
  }


  public async Task EnableVideoStreamAsync(bool enable)
  {
    if (!IsConnected || _device == null || _cancellationTokenSource is { IsCancellationRequested: true })
      return;

    var message = new SdcpRequest<VideoStreamData>
    {
      Id = _device.Id,
      Topic = $"sdcp/request/{_device.Data.MainboardID}",
      Data =
      {
        Cmd = EnableDisableVideoStreamCommand,
        RequestID = $"{_nextRequestId:X016}",
        From = (int)SdcpFrom.Pc,
        MainboardID = _device.Data.MainboardID,
        Data = { Enable = enable ? 1 : 0}
      }
    };

    _requestTimeStamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    message.Data.TimeStamp = _requestTimeStamp;
    _nextRequestId++;

    await SendString(_webSocket!, JsonSerializer.Serialize(message), _cancellationTokenSource!.Token).ConfigureAwait(false);
  }


  private static Task SendString(ClientWebSocket ws, string data, CancellationToken token)
  {
    byte[] encoded = Encoding.UTF8.GetBytes(data);
    var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
    return ws.SendAsync(buffer, WebSocketMessageType.Text, true, token);
  }


  private async Task ReceiveMessages(CancellationToken token)
  {
    if (_webSocket == null)
      return;

    try
    {
      using var ms = new MemoryStream();
      while (!token.IsCancellationRequested &&
             _webSocket.State == WebSocketState.Open)
      {
        WebSocketReceiveResult result;
        do
        {
          var messageBuffer = WebSocket.CreateClientBuffer(1024, 16);
          result = await _webSocket.ReceiveAsync(messageBuffer, token);
          ms.Write(messageBuffer.Array!, messageBuffer.Offset, result.Count);
        }
        while (!token.IsCancellationRequested && !result.EndOfMessage);

        if (token.IsCancellationRequested)
          return;

        if (result.MessageType == WebSocketMessageType.Text)
          HandleMessage(Encoding.UTF8.GetString(ms.ToArray()));

        ms.SetLength(0);
      }
    }
    catch (OperationCanceledException)
    {
      Debug.WriteLine("[WS] Operation canceled.");
    }
    catch (InvalidOperationException)
    {
      Debug.WriteLine("[WS] Tried to receive message while already reading one.");
    }
    catch
    {
      Debug.WriteLine("[WS] Unexpected exception.");
    }
  }

  private void HandleMessage(string jsonMessage)
  {
    try
    {
      _logger.LogDebug("WebSocket Message: {Message}", jsonMessage);
      Debug.WriteLine($"WebSocket Message: {jsonMessage}");
      var baseMessage = JsonSerializer.Deserialize<SdcpMessage>(jsonMessage);
      if (baseMessage!.Topic.StartsWith("sdcp/response"))
      {
        HandleResponseMessage(jsonMessage);
      }
      else if (baseMessage.Topic.StartsWith("sdcp/status"))
      {
        HandleStatusMessage(jsonMessage);
      }
      else if (baseMessage.Topic.StartsWith("sdcp/attributes"))
      {
        HandleAttributesMessage(jsonMessage);
      }
      else if (baseMessage.Topic.StartsWith("sdcp/error"))
      {
        HandleErrorMessage(jsonMessage);
      }
      else if (baseMessage.Topic.StartsWith("sdcp/notice"))
      {
        HandleNoticeMessage(jsonMessage);
      }
      else
      {
        Debug.WriteLine("Unsupported message type");
      }
    }
    catch (Exception e)
    {
      Debug.WriteLine(e);

    }
  }


  private void HandleNoticeMessage(string jsonMessage)
  {
    _logger.LogInformation("Notice: {Notice}", jsonMessage);
  }


  private void HandleErrorMessage(string jsonMessage)
  {
    _logger.LogWarning("Error: {Error}", jsonMessage);
  }


  private void HandleAttributesMessage(string jsonMessage)
  {
    _logger.LogInformation("Attributes: {Attributes}", jsonMessage);
  }


  private void HandleStatusMessage(string jsonMessage)
  {
    var status = JsonSerializer.Deserialize<StatusMessage>(jsonMessage)!;
    _logger.LogDebug("Status: {Status}", status.Status.CurrentStatus);

    _timer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
    //_lastStatusUpdateTimestamp = _timeProvider.GetTimestamp();

    WeakReferenceMessenger.Default.Send(new DeviceStatusMessage(status));
  }


  private void HandleResponseMessage(string jsonMessage)
  {
    var message = JsonSerializer.Deserialize<SdcpResponse>(jsonMessage)!;
    switch (message.Data.Cmd)
    {
      case EnableDisableVideoStreamCommand:
        var videoResponse = JsonSerializer.Deserialize<SdcpResponse<VideoStreamResponse>>(jsonMessage)!;
        if ((VideoStreamAck)videoResponse.Data.Data.Ack == VideoStreamAck.Success)
        {
          VideoUrl = videoResponse.Data.Data.VideoUrl;
        }

        break;

      default:
        _logger.LogError("Response contains unsupported command {Command}", message.Data.Cmd);
        break;
    }
  }
}