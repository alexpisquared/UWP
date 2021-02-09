using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Media.SpeechSynthesis;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdpSupport
{
  public sealed partial class MainPage : Page
  {
    const int _periodSec = 60, _till=20;
    readonly MediaElement _mediaplayer = new MediaElement();
    readonly DisplayRequest _dr = new DisplayRequest();
    readonly DispatcherTimer _timer = new DispatcherTimer();
    DateTime _since;
    Point _prevPosition;
    int _sound = 0;
    SpeechSynthesizer _synth = new SpeechSynthesizer();

    public MainPage()
    {
      InitializeComponent();
      CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
      _timer.Tick += onTick;
      _timer.Interval = TimeSpan.FromSeconds(_periodSec);
      _timer.Start();
    }

    async void Page_Loaded(object s, RoutedEventArgs e)
    {
      await Task.Delay(999); onStart(s, e); _prevPosition = CoreWindow.GetForCurrentThread().PointerPosition;
      _synth.Voice = SpeechSynthesizer.AllVoices.LastOrDefault(gender => gender.Gender == VoiceGender.Female) ?? SpeechSynthesizer.DefaultVoice;
      foreach (VoiceInformation vi in SpeechSynthesizer.AllVoices)
      {
        tbkLog.Text += $"{vi.Description}\r\n";
        Debug.WriteLine(tbkLog.Text);
      }

      tbkLog.Text += $"==> {_synth.Voice.Description}\r\n";
    }
    async void onTick(object s, object e)
    {
      if (DateTime.Now.Hour >= _till && checkBox.IsChecked == true)
        setDR(false);

      if (CoreWindow.GetForCurrentThread().PointerPosition == _prevPosition)
      {
        Window.Current.CoreWindow.PointerPosition = new Point(_prevPosition.X + 1, _prevPosition.Y + 1);
        if (CoreWindow.GetForCurrentThread().PointerPosition == _prevPosition)
        {
          ElementSoundPlayer.State = ElementSoundPlayerState.On;
          var sss = (ElementSoundKind)((_sound++) % (1 + (int)ElementSoundKind.GoBack));
          tbkLog.Text += $"{DateTime.Now:HH:mm:ss}  Unhinged\t{_prevPosition}\t{sss} \r\n";
          await readText("I need focus");
          ElementSoundPlayer.Play(ElementSoundKind.Invoke);
          Window.Current.CoreWindow.PointerPosition = new Point(_prevPosition.X + 1, _prevPosition.Y + 1);
        }
        else
          tbkLog.Text += $"{DateTime.Now:HH:mm:ss}  Fixed\t{_prevPosition} \r\n";
      }
      else
      {
        tbkLog.Text += $"{DateTime.Now:HH:mm:ss}  Busy\t{_prevPosition} \r\n";
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
      {
        _dr.RequestActive();
        tbkLog.Text = $"{DateTime.Now:HH:mm:ss}\r\n";
      }
      else
      {
        _dr.RequestRelease();
      }

      tbkBig.Text = isOn ? $"On {(_since = DateTime.Now):HH:mm} ÷ {_till}:00" : $"Was On for {(DateTime.Now - _since):hh\\:mm}";
      ApplicationView.GetForCurrentView().Title = isOn ? $"{(_since = DateTime.Now):HH:mm}···" : $"Off";

      //buttonStart.Visibility = v ? Visibility.Collapsed : Visibility.Visible;
      //buttonStop.Visibility = !v ? Visibility.Collapsed : Visibility.Visible;
    }
    async Task readText(string mytext)
    {
      var ssml = $"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>{mytext}</speak>";
      var stream = await _synth.SynthesizeSsmlToStreamAsync(ssml);
      _mediaplayer.SetSource(stream, stream.ContentType);
      _mediaplayer.Play();
    }
  }
}