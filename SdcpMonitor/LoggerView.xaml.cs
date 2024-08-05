using System.ComponentModel;
using MahApps.Metro.Controls;

namespace SdcpMonitor
{
  /// <summary>
  /// Interaction logic for LoggerView.xaml
  /// </summary>
  public partial class LoggerView : MetroWindow
  {
    private readonly LoggerViewModel _viewModel;

    public LoggerView(LoggerViewModel viewModel)
    {
      _viewModel = viewModel;
      InitializeComponent();
      DataContext = viewModel;
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      _viewModel.IsActive = true;
    }


    protected override void OnClosing(CancelEventArgs e)
    {
      if (!App.Current.IsClosing)
      {
        Hide();
        e.Cancel = true;
      }

      base.OnClosing(e);
    }


    protected override void OnClosed(EventArgs e)
    {

      _viewModel.IsActive = false;

      base.OnClosed(e);
    }
  }
}
