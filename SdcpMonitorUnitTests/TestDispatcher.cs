using SdcpMonitor;

namespace SdcpMonitorUnitTests;

public class TestDispatcher : IDispatcher
{
  public async Task InvokeAsync(Action callback)
  {
    await Task.Run(callback).ConfigureAwait(false);
  }


  public async Task InvokeAsync(Func<Task> callback)
  {
    await callback.Invoke().ConfigureAwait(false);
  }
}