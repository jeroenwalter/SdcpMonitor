// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
namespace Sdcp;

public class SdcpRequest : SdcpRequest<MessageData>
{ }

public class SdcpRequest<T>
: SdcpMessage
  where T : MessageData, new()
{
  public class RequestData
  {
    public long Cmd { get; set; } = 0;
    public T Data { get; set; } = new();
    public string RequestID { get; set; } = "";
    public ulong TimeStamp { get; set; } = 0;
    public int From { get; set; } = 0;
    public string MainboardID { get; set; } = "";
  }
  
  public RequestData Data { get; set; } = new();
}