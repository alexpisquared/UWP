using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
