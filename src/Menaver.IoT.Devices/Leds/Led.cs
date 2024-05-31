using System.Drawing;

namespace Menaver.IoT.Devices.Leds;

public class Led
{
    public Led(byte pin, Color color, bool enabled)
    {
        Pin = pin;
        Color = color;
        Enabled = enabled;
    }

    public byte Pin { get; set; }
    public Color Color { get; set; }
    public bool Enabled { get; set; }
}