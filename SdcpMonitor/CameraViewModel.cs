using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Logging;
using Sdcp;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace SdcpMonitor
{
  public sealed partial class CameraViewModel : ObservableRecipient, IDisposable
  {
    private readonly ILogger<CameraViewModel> _logger;
    private readonly LibVLC _libVlc = new(["--rtsp-timeout=300 --verbose=2"]);
    private IDeviceCommunication? _deviceCommunication = null;
    
    [ObservableProperty] private string _overlayText = "";
    [ObservableProperty] private string _title = "Camera";

    public MediaPlayer MediaPlayer { get; }

    public CameraViewModel(ILogger<CameraViewModel> logger)
    {
      _logger = logger;

      _logger.LogDebug("Camera window opened");

      MediaPlayer = new MediaPlayer(_libVlc);
      MediaPlayer.EncounteredError += MediaPlayerOnEncounteredError;
      _libVlc.Log += LibVlcOnLog;
    }

    private IAsyncRelayCommand? _playVideoStreamCommand;

    public IAsyncRelayCommand PlayVideoStreamCommand =>
      _playVideoStreamCommand ??= new AsyncRelayCommand(PlayVideoStreamAsync);

    private IAsyncRelayCommand? _stopVideoStreamCommand;

    public IAsyncRelayCommand StopVideoStreamCommand =>
      _stopVideoStreamCommand ??= new AsyncRelayCommand(StopVideoStreamAsync);

    

    protected override void OnActivated()
    {
      base.OnActivated();

      WeakReferenceMessenger.Default.Register<CameraOverlayTextChangedMessage>(this, (viewModel, overlayText) =>
      {
        var self = (CameraViewModel)viewModel;
        Application.Current.Dispatcher.InvokeAsync<string>(() => self.OverlayText = overlayText.Value);
      });
    }

    protected override void OnDeactivated()
    {
      MediaPlayer.Stop();

      base.OnDeactivated();

    }


    private void LibVlcOnLog(object? sender, LogEventArgs e)
    {
      _logger.LogDebug(e.FormattedLog);
      Debug.WriteLine(e.FormattedLog);
    }

    private void MediaPlayerOnEncounteredError(object? sender, EventArgs e)
    {
      _logger.LogDebug(e.ToString());
      Debug.WriteLine(e);
    }



    private async Task StopVideoStreamAsync()
    {
      if (MediaPlayer.IsPlaying)
        MediaPlayer.Stop();

      if (_deviceCommunication == null)
        return;

      await _deviceCommunication.EnableVideoStreamAsync(false).ConfigureAwait(true);
    }


    private async Task PlayVideoStreamAsync()
    {
      if (_deviceCommunication == null)
        return;

      if (MediaPlayer.IsPlaying)
        return;

      await _deviceCommunication.EnableVideoStreamAsync(true).ConfigureAwait(true);

      await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(true);
      if (string.IsNullOrEmpty(_deviceCommunication.VideoUrl))
        return;


      //var rtspUrl = "rtsp://127.0.0.1:8554/video";
      //var bunny = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
      //using var media = new Media(_libVlc, new Uri(rtspUrl));
      using var media = new Media(_libVlc, new Uri(_deviceCommunication.VideoUrl));
      //var config = new MediaConfiguration();
      //config.EnableHardwareDecoding = true;
      //media.AddOption(":rtsp-tcp");
      //media.AddOption(":rtsp-mcast");
      //media.AddOption(":rtsp-timeout=300");
      //media.AddOption(config);
      if (!MediaPlayer.Play(media)) Debug.WriteLine("Failed to open video stream");
    }


    public void Dispose()
    {
      MediaPlayer.Dispose();
      _libVlc.Dispose();
    }
  }
}
