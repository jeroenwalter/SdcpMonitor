using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using MahApps.Metro.Controls;

namespace SdcpMonitor
{
  /// <summary>
  /// Interaction logic for AboutView.xaml
  /// </summary>
  public partial class AboutView : MetroWindow
  {
    public AboutView()
    {
      InitializeComponent();
      if (!DesignerProperties.GetIsInDesignMode(this))
        SubscribeToAllHyperlinks(this);
    }

    private static void SubscribeToAllHyperlinks(DependencyObject flowDocument)
    {
      var hyperlinks = GetVisuals(flowDocument).OfType<Hyperlink>();
      foreach (var link in hyperlinks)
      {
        link.RequestNavigate += OnRequestNavigate;
      }
    }


    private static IEnumerable<DependencyObject> GetVisuals(DependencyObject root)
    {
      foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
      {
        yield return child;
        foreach (var descendants in GetVisuals(child))
          yield return descendants;
      }
    }

    private static void OnRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      var sInfo = new ProcessStartInfo(e.Uri.AbsoluteUri)
      {
        UseShellExecute = true
      };
      Process.Start(sInfo);
      e.Handled = true;
    }
  }
}
