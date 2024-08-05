namespace Sdcp;

public class SdcpResponse : SdcpResponse<MessageData>
{ }

public class SdcpResponse<T>
: SdcpMessage
  where T : MessageData, new()
{
  public class ResponseData
  {
    public long Cmd { get; set; } = 0;
    public T Data { get; set; } = new();
    public string RequestID { get; set; } = "";
    public ulong TimeStamp { get; set; } = 0;
    public string MainboardID { get; set; } = "";
  }

  public ResponseData Data { get; set; } = new();
}