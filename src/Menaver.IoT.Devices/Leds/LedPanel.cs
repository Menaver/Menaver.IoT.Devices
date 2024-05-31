using System.Device.Gpio;
using System.Drawing;

namespace Menaver.IoT.Devices.Leds;

/// <summary>
///     Abstraction of interaction with basic LED panel.
///     Navigate to <see href="https://en.wikipedia.org/wiki/Light-emitting_diode">LED</see>
///     for more details about the module.
/// </summary>
public interface ILedPanel : IDisposable
{
    void Set(int ledPin);
    void SetAll(Color ledColor);
    void SetAll();
    void Reset(int ledPin);
    void ResetAll(Color ledColor);
    void ResetAll();
    void Toggle(int ledPin);
    void ToggleAll(Color ledColor);
    void ToggleAll();
}

/// <summary>
///     Implementation of interaction with basic LED panel.
///     Navigate to <see href="https://en.wikipedia.org/wiki/Light-emitting_diode">LED</see>
///     for more details about the module.
/// </summary>
public class LedPanel : ILedPanel
{
    private readonly GpioController _controller;
    private readonly IList<Led> _leds;

    public LedPanel(IList<Led> leds)
    {
        _leds = leds;
        _controller = new GpioController();

        foreach (var led in _leds)
        {
            _controller.OpenPin(led.Pin, PinMode.Output);
            _controller.Write(led.Pin, led.Enabled ? PinValue.High : PinValue.Low);
        }
    }

    public void Set(int ledPin)
    {
        var led = _leds.FirstOrDefault(x => x.Pin == ledPin);

        if (led == null)
        {
            throw new ArgumentException("LED pin isn't defined.");
        }

        EnablePin(led);
    }

    public void SetAll(Color ledColor)
    {
        var leds = _leds.Where(x => x.Color == ledColor).ToList();

        if (!leds.Any())
        {
            throw new ArgumentException("LED color isn't defined.");
        }

        foreach (var led in leds)
        {
            EnablePin(led);
        }
    }

    public void SetAll()
    {
        foreach (var led in _leds)
        {
            EnablePin(led);
        }
    }

    public void Reset(int ledPin)
    {
        var led = _leds.FirstOrDefault(x => x.Pin == ledPin);

        if (led == null)
        {
            throw new ArgumentException("LED pin isn't defined.");
        }

        DisablePin(led);
    }

    public void ResetAll(Color ledColor)
    {
        var leds = _leds.Where(x => x.Color == ledColor).ToList();

        if (!leds.Any())
        {
            throw new ArgumentException("LED color isn't defined.");
        }

        foreach (var led in leds)
        {
            DisablePin(led);
        }
    }

    public void ResetAll()
    {
        foreach (var led in _leds)
        {
            DisablePin(led);
        }
    }

    public void Toggle(int ledPin)
    {
        var led = _leds.FirstOrDefault(x => x.Pin == ledPin);

        if (led == null)
        {
            throw new ArgumentException("LED pin isn't defined.");
        }

        TogglePin(led);
    }

    public void ToggleAll(Color ledColor)
    {
        var leds = _leds.Where(x => x.Color == ledColor).ToList();

        if (!leds.Any())
        {
            throw new ArgumentException("LED color isn't defined.");
        }

        foreach (var led in leds)
        {
            TogglePin(led);
        }
    }

    public void ToggleAll()
    {
        foreach (var led in _leds)
        {
            TogglePin(led);
        }
    }

    private void EnablePin(Led led)
    {
        _controller.Write(led.Pin, PinValue.High);
        led.Enabled = true;
    }

    private void DisablePin(Led led)
    {
        _controller.Write(led.Pin, PinValue.Low);
        led.Enabled = false;
    }

    private void TogglePin(Led led)
    {
        led.Enabled = !led.Enabled;
        _controller.Write(led.Pin, led.Enabled ? PinValue.High : PinValue.Low);
    }

    #region IDisposable

    ~LedPanel()
    {
        Dispose(false);
    }

    private bool _disposedValue;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _controller.Dispose();
            }

            _disposedValue = true;
        }
    }

    #endregion
}