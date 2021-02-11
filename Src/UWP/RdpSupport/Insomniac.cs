using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Display;

namespace RdpSupport
{
  public class Insomniac
  {
    readonly DisplayRequest _dr = new DisplayRequest();
    bool _isPresenting;

    public bool IsPresenting
    {
      get => _isPresenting; set
      {
        setDR(_isPresenting = value);
      }
    }

    public void RequestActive() => IsPresenting = true;
    public void RequestRelease() => IsPresenting = false;


    void setDR(bool isOn)
    {
      if (isOn)
      {
        _dr.RequestActive();
      }
      else
      {
        _dr.RequestRelease();
      }
    }

  }
}
