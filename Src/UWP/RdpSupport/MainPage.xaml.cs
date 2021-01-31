using System;
using System.Threading.Tasks;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdpSupport
{
  public sealed partial class MainPage : Page
  {
    DisplayRequest disp = new DisplayRequest();
    DateTime _since;

    public MainPage() => InitializeComponent();

    void onStart(object s, RoutedEventArgs e) { disp.RequestActive(); checkBox.IsChecked = true; dd(checkBox.IsChecked == true); textBlock.Text = $"Since {(_since = DateTime.Now):HH:mm}"; }
    void onStop(object se, RoutedEventArgs e) { disp.RequestRelease(); checkBox.IsChecked = false; dd(checkBox.IsChecked == true); textBlock.Text = $"For {(DateTime.Now - _since):hh\\:mm}"; }

    async void Page_Loaded(object s, RoutedEventArgs e)
    {
      await Task.Delay(999);
      onStart(s, e);
    }

    void dd(bool v)
    {
      //buttonStart.Visibility = v ? Visibility.Collapsed : Visibility.Visible;
      //buttonStop.Visibility = !v ? Visibility.Collapsed : Visibility.Visible;
    }
  }
}
