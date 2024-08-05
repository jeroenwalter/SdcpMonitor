using Microsoft.Extensions.Logging;
using Sdcp;

namespace SdcpUnitTests
{
  public class DeviceDiscoveryTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Start()
    {
      // Arrange
      var deviceDiscovery = new DeviceDiscovery(NSubstitute.Substitute.For<ILogger<DeviceDiscovery>>());

      // Act
      //deviceDiscovery.Start(TimeSpan.FromSeconds(5));

      // Assert
      //Assert.That(result, Is.Not.Empty);
    }
  }
}