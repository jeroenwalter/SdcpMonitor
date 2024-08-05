using System.ComponentModel;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sdcp;
using SdcpMonitor;

namespace SdcpMonitorUnitTests
{
  public class MainWindowViewModelTests
  {
    private readonly MainViewModel _viewModel = new(Substitute.For<ILogger<MainViewModel>>(),
      Substitute.For<INavigationService>(),
      Substitute.For<Settings>());

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ObservablePropertyChanged()
    {
      // Arrange
      _viewModel.PropertyChanged += ViewModelOnPropertyChanged;

      var propertyName = "";
      
      void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
      {
        propertyName = e.PropertyName;
      }

      // Act
      _viewModel.Status = "AA";

      // Assert
      Assert.That(propertyName, Is.EqualTo(nameof(SdcpMonitor.MainViewModel.Status)));

      _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
    }
  }
}