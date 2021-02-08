using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace RdpSupport
{
  public sealed partial class MainPage : Page
  {
    readonly DisplayRequest _dr = new DisplayRequest();
    DateTime _since;
    DispatcherTimer _timer = new DispatcherTimer();

    public MainPage()
    {
      InitializeComponent();
      CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
      _timer.Tick += _timer_Tick;
      _timer.Interval = TimeSpan.FromSeconds(90);
      _timer.Start();
    }

    void _timer_Tick(object sender, object e)
    {
      if (DateTime.Now.Hour >= 20 && checkBox.IsChecked == true)
        setDR(false);
    }

    async void Page_Loaded(object s, RoutedEventArgs e) { await Task.Delay(999); onStart(s, e); }

    void onStart(object s, RoutedEventArgs e) => setDR(true);
    void onStop(object se, RoutedEventArgs e) => setDR(false);
    void onMove(object s, RoutedEventArgs e)
    {
      var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
      Debug.WriteLine($"** XY: {pointerPosition}");

      Window.Current.CoreWindow.PointerPosition = new Point(pointerPosition.X + 100, pointerPosition.Y);

      //Cursor = new Cursor(Cursor.Current.Handle);
      //Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
      //Cursor.Clip = new Rectangle(this.Location, this.Size);
    }

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
