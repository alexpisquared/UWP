using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdpSupport
{
  public sealed partial class MainPage : Page
  {
    readonly DisplayRequest _dr = new DisplayRequest();
    readonly DispatcherTimer _timer = new DispatcherTimer();
    DateTime _since;
    Point _prevPosition;

    public MainPage()
    {
      InitializeComponent();
      CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
      _timer.Tick += onTick;
      _timer.Interval = TimeSpan.FromSeconds(090);
      _timer.Start();
      textBloc3.Text = $"{DateTime.Now:HH:mm:ss}\r\n";
    }

    async void Page_Loaded(object s, RoutedEventArgs e) { await Task.Delay(999); onStart(s, e); _prevPosition = CoreWindow.GetForCurrentThread().PointerPosition; }
    void onTick(object s, object e)
    {
      if (DateTime.Now.Hour >= 20 && checkBox.IsChecked == true)
        setDR(false);

      var psn = CoreWindow.GetForCurrentThread().PointerPosition;
      if (psn == _prevPosition)
      {
        textBloc3.Text += $"{DateTime.Now:HH:mm:ss}  Idle\t{_prevPosition} \r\n";
        Window.Current.CoreWindow.PointerPosition = new Point(_prevPosition.X + 1, _prevPosition.Y + 1);
      }
      else
      {
        textBloc3.Text += $"{DateTime.Now:HH:mm:ss}  Busy\t{_prevPosition} \r\n";
      }
      _prevPosition = CoreWindow.GetForCurrentThread().PointerPosition;
      Debug.WriteLine($"** XY: {_prevPosition}");
    }
    void onStart(object s, RoutedEventArgs e) => setDR(true);
    void onStop(object se, RoutedEventArgs e) => setDR(false);
    void onMove(object s, RoutedEventArgs e) { }
    void onExit(object s, RoutedEventArgs e) { }

    void setDR(bool isOn)
    {
      checkBox.IsChecked = isOn;
      if (isOn)
        _dr.RequestActive();
      else
        _dr.RequestRelease();

      textBlock.Text = isOn ? $"On since {(_since = DateTime.Now):HH:mm}" : $"Was On for {(DateTime.Now - _since):hh\\:mm}";
      ApplicationView.GetForCurrentView().Title = isOn ? $"{(_since = DateTime.Now):HH:mm}···" : $"Off";

      //buttonStart.Visibility = v ? Visibility.Collapsed : Visibility.Visible;
      //buttonStop.Visibility = !v ? Visibility.Collapsed : Visibility.Visible;
    }
  }
}
