using System.Drawing;
using Menaver.IoT.Devices.Leds;

namespace Menaver.IoT.Devices.Tests.Programs;

internal class LedBlinking
{
    private static readonly List<Led> _leds = new()
    {
        new Led(16, Color.Red, false),
        new Led(21, Color.Green, false),
        new Led(20, Color.Yellow, false)
    };

    public static async Task<int> RunAsync()
    {
        using var ledPanel = new LedPanel(_leds);

        ledPanel.SetAll();

        await Task.Delay(300);

        ledPanel.ResetAll();

        await Task.Delay(300);

        ledPanel.SetAll(Color.Red);

        await Task.Delay(300);

        ledPanel.ResetAll(Color.Red);

        await Task.Delay(300);

        ledPanel.SetAll(Color.Green);

        await Task.Delay(300);

        ledPanel.ResetAll(Color.Green);

        await Task.Delay(300);

        ledPanel.SetAll(Color.Yellow);

        await Task.Delay(300);

        ledPanel.ResetAll(Color.Yellow);

        while (true)
        {
            foreach (var led in _leds)
            {
                ledPanel.Toggle(led.Pin);

                await Task.Delay(100);
            }
        }
    }
}