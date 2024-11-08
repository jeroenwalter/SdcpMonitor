using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Sdcp;

namespace SdcpUnitTests;

public class DeviceCommunicationTests
{
  private readonly DeviceCommunication _deviceCommunication;
  private readonly ILogger<DeviceCommunication> _loggerMock = Substitute.For<ILogger<DeviceCommunication>>();
  private readonly FakeTimeProvider _fakeTimeProvider = new ();
  
  public DeviceCommunicationTests()
  {
    _deviceCommunication = new DeviceCommunication(_loggerMock, _fakeTimeProvider);

  }


  [Test]
  public void OnConnected_StartsTimer()
  {
    
  }
}