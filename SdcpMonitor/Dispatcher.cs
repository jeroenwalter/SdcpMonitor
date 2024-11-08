using System.Windows;

namespace SdcpMonitor;

public class Dispatcher : IDispatcher
{
  public async Task InvokeAsync(Action callback)
  {
    await Application.Current.Dispatcher.InvokeAsync(callback);
  }

  public async Task InvokeAsync(Func<Task> callback)
  {
    await Application.Current.Dispatcher.InvokeAsync(callback);
  }
}