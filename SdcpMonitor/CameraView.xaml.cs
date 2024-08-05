using MahApps.Metro.Controls;

namespace SdcpMonitor
{

  public partial class CameraView : MetroWindow
  {
    private readonly CameraViewModel _viewModel;

    public CameraView(CameraViewModel viewModel)
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


    protected override void OnClosed(EventArgs e)
    {
      _viewModel.IsActive = false;

      base.OnClosed(e);

      VideoView.Dispose();
    }
  }
}
