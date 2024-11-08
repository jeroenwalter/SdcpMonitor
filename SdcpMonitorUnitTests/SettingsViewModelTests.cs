using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sdcp;
using SdcpMonitor;

namespace SdcpMonitorUnitTests;

public class SettingsViewModelTests
{
  private readonly ILogger<SettingsViewModel> _loggerMock = Substitute.For<ILogger<SettingsViewModel>>();
  private readonly IDeviceDiscovery _deviceDiscoveryMock = Substitute.For<IDeviceDiscovery>();
  private readonly TestDispatcher _testDispatcher = new();


  [SetUp]
  public void Setup()
  {
  }


  [Test]
  public void PrinterIsInitializedFromSettings()
  {
    // Arrange
    var settings = new Settings { Printer = new Printer { Id = "Id 1", Ip = "1.1.1.1", Name = "Printer 1" } };

    // Act
    SettingsViewModel viewModel = new(_loggerMock, settings, _deviceDiscoveryMock, _testDispatcher);

    // Assert
    Assert.That(viewModel.Printer, Is.EqualTo(settings.Printer.ToString()));
  }


  
  [Test]
  public void OnDeactivated_PrinterIsSavedToSettings()
  {
    // Arrange
    var settings = new Settings { Printer = new Printer { Id = "Id 1", Ip = "1.1.1.1", Name = "Printer 1" } };
    
    SettingsViewModel viewModel = new(_loggerMock, settings, _deviceDiscoveryMock, _testDispatcher);

    viewModel.IsActive = true;

    // Act
    viewModel.IsActive = false;

    // Assert
    Assert.That(viewModel.Printer, Is.EqualTo(settings.Printer.ToString()));
  }


  [Test]
  public async Task StartDiscoveryDoesNotClearPrinter()
  {
    var settings = new Settings { Printer = new Printer { Id = "Id 1", Ip = "1.1.1.1", Name = "Printer 1" } };

    SettingsViewModel viewModel = new(_loggerMock, settings, _deviceDiscoveryMock, _testDispatcher);

    await viewModel.StartDiscoveryCommand.ExecuteAsync(null).ConfigureAwait(false);
  }


  [Test]
  public void StopDiscoveryDoesNotClearPrinter()
  {

  }


  [Test]
  public void DeviceDiscoveredMessage_PrinterIsSetToDiscoveredDevice()
  {
    // Arrange
    var settings = new Settings();
    Assume.That(settings.Printer, Is.Null);
    SettingsViewModel viewModel = new(_loggerMock, settings, _deviceDiscoveryMock, _testDispatcher);

    Assume.That(WeakReferenceMessenger.Default.IsRegistered<DeviceDiscoveredMessage>(viewModel), Is.False);
    viewModel.IsActive = true;
    Assume.That(WeakReferenceMessenger.Default.IsRegistered<DeviceDiscoveredMessage>(viewModel), Is.True);

    // Act
    var device = new Device
    {
      Data = { MachineName = "MachineName", Name = "Name", MainboardID = "MainBoardId", MainboardIP = "1.2.3.4" }
    };
    WeakReferenceMessenger.Default.Send(new DeviceDiscoveredMessage(device));

    // Assert
    //await _dispatcherMock.Received(0).InvokeAsync(Arg.Any<Action>()).ConfigureAwait(false);
    //await _dispatcherMock.Received(1).InvokeAsync(Arg.Any<Func<Task>>()).ConfigureAwait(false);

    Assert.That(viewModel.Printer, Is.EqualTo($"{device.Data.Name} ({device.Data.MainboardIP})"));
    Assert.That(settings.Printer, Is.Not.Null);
    Assert.That(settings.Printer.Id, Is.EqualTo(device.Data.MainboardID));
    Assert.That(settings.Printer.Name, Is.EqualTo(device.Data.Name));
    Assert.That(settings.Printer.Ip, Is.EqualTo(device.Data.MainboardIP));
  }


  [Test]
  public void DeletePrinterDeletesPrinterFromPrintersList()
  {

  }


}