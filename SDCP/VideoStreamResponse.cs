namespace Sdcp;

public class VideoStreamResponse : MessageData
{
  public int Ack { get; set; } = 0; // 0: Success 1: Exceeded maximum simultaneous streaming limit 2: Camera does not exist 3: Unknown error
  public string VideoUrl { get; set; } = ""; //When opening the video stream, return the RTSP protocol address
}