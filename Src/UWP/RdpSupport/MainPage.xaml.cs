using System;
using System.Collections.Generic;
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
#if DEBUG
    const int _periodSec = 1, _till = 20;
    int _dx = 50, _dy = 50;
#else
    const int _periodSec = 60, _till = 20;
    int _dx = 1, _dy = 1;
#endif
    readonly MediaElement _mediaplayer = new MediaElement();
    readonly Insomniac _dr = new Insomniac();
    readonly DispatcherTimer _timer = new DispatcherTimer();
    readonly SpeechSynthesizer _synth = new SpeechSynthesizer();
    readonly IReadOnlyList<VoiceInformation> _av = SpeechSynthesizer.AllVoices;
    DateTime _since;
    Point _prevPosition;
    int _voice = 0;

    public MainPage()
    {
      InitializeComponent();
      CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
      _timer.Tick += onTick;
      _timer.Interval = TimeSpan.FromSeconds(_periodSec);
      _timer.Start();
    }

    async void onLoaded(object s, RoutedEventArgs e)
    {
      await Task.Delay(999);
      onStrt(s, e);
      _prevPosition = CoreWindow.GetForCurrentThread().PointerPosition;

      _synth.Voice = _av.LastOrDefault(gender => gender.Gender == VoiceGender.Female) ?? SpeechSynthesizer.DefaultVoice;
      foreach (var vi in _av)
      {
        tbkLog.Text += $"{vi.Description.Replace("-", "\t")}\r\n";
        Debug.WriteLine(tbkLog.Text);
      }

      ElementSoundPlayer.State = ElementSoundPlayerState.On;
    }
    async void onTick(object s, object e)
    {
      if (DateTime.Now.Hour >= _till && checkBox.IsChecked == true)
        setDR(false);

      if (CoreWindow.GetForCurrentThread().PointerPosition == _prevPosition)
      {
        Window.Current.CoreWindow.PointerPosition = new Point(_prevPosition.X + _dx, _prevPosition.Y + _dy);
        if (CoreWindow.GetForCurrentThread().PointerPosition == _prevPosition)
        {
          _dx = -_dx;
          Window.Current.CoreWindow.PointerPosition = new Point(_prevPosition.X + _dx, _prevPosition.Y + _dy);
          if (CoreWindow.GetForCurrentThread().PointerPosition == _prevPosition)
          {
            _dy = -_dy;
            Window.Current.CoreWindow.PointerPosition = new Point(_prevPosition.X + _dx, _prevPosition.Y + _dy);
          }
          if (CoreWindow.GetForCurrentThread().PointerPosition == _prevPosition)
          {
            _synth.Voice = _av[(_voice++) % _av.Count];
            await readText("I need focus");
            tbkLog.Text += $"{DateTime.Now:HH:mm:ss}  Focus!   \t{_prevPosition,12}\t{_synth.Voice.Description} \r\n";
            Window.Current.CoreWindow.PointerPosition = new Point(_prevPosition.X + 1, _prevPosition.Y + 1);
          }
        }
        else
          tbkLog.Text += $"{DateTime.Now:HH:mm:ss}  Remedied.\t{_prevPosition,12} \r\n";
      }
      else
      {
        tbkLog.Text += $"{DateTime.Now:HH:mm:ss}  No need. \t{_prevPosition,12} \r\n";
      }

      _prevPosition = CoreWindow.GetForCurrentThread().PointerPosition;
      Debug.WriteLine($"** XY: {_prevPosition}");
    }
    void onStrt(object s, RoutedEventArgs e) => setDR(true);
    void onStop(object s, RoutedEventArgs e) => setDR(false);
    void onMove(object s, RoutedEventArgs e) => ElementSoundPlayer.Play(ElementSoundKind.Show);
    void onExit(object s, RoutedEventArgs e) => ElementSoundPlayer.Play(ElementSoundKind.Hide);

    void setDR(bool isOn)
    {
      checkBox.IsChecked = isOn;
      if (isOn)
      {
        _dr.RequestActive();
        tbkLog.Text = $"{DateTime.Now:HH:mm:ss}\r\n";
        ElementSoundPlayer.Play(ElementSoundKind.Show);
      }
      else
      {
        _dr.RequestRelease();
        ElementSoundPlayer.Play(ElementSoundKind.Hide);
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