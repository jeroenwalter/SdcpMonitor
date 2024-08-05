using MahApps.Metro.Controls;

namespace SdcpMonitor;

public partial class SettingsView : MetroWindow
{
  private readonly SettingsViewModel _viewModel;

  public SettingsView(SettingsViewModel viewModel)
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
  }
}
