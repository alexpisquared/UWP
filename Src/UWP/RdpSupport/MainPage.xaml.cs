using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdpSupport
{
  public sealed partial class MainPage : Page
  {
    DisplayRequest _disp = new DisplayRequest();
    DateTime _since;

    public MainPage()
    {
      InitializeComponent();
      CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
    }

    async void Page_Loaded(object s, RoutedEventArgs e) { await Task.Delay(999); onStart(s, e); }

    void onStart(object s, RoutedEventArgs e) { _disp.RequestActive(); checkBox.IsChecked = true; dd(checkBox.IsChecked == true); textBlock.Text = $"Since {(_since = DateTime.Now):HH:mm}"; }
    void onStop(object se, RoutedEventArgs e) { _disp.RequestRelease(); checkBox.IsChecked = false; dd(checkBox.IsChecked == true); textBlock.Text = $"For {(DateTime.Now - _since):hh\\:mm}"; }
    void onExit(object se, RoutedEventArgs e) {; }


    void dd(bool v)
    {
      //buttonStart.Visibility = v ? Visibility.Collapsed : Visibility.Visible;
      //buttonStop.Visibility = !v ? Visibility.Collapsed : Visibility.Visible;
    }
  }
}
