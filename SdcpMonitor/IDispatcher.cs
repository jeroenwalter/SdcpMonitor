namespace SdcpMonitor;

public interface IDispatcher
{
  Task InvokeAsync(Action callback);
  Task InvokeAsync(Func<Task> callback);
}