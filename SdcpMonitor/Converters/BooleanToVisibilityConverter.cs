#nullable disable

using System.Windows;

namespace SdcpMonitor.Converters;

public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
{
  public BooleanToVisibilityConverter() :
    base(Visibility.Visible, Visibility.Collapsed)
  { }
}
