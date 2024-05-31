using Menaver.IoT.Devices.Displays;

namespace Menaver.IoT.Devices.Tests.Programs;

internal class LcDisplay
{
    public static async Task<int> RunAsync()
    {
        var lcd = new LcdPcx857x(
            Pcx857xDevice.Pcf8574,
            sdaPin: 2,
            sclPin: 3,
            displayRowCount: 2,
            displayColumnCount: 16);

        while (true)
        {
            await lcd.WriteTextAsync("Hello, Menaver!");

            await Task.Delay(1000);

            await lcd.WriteTextAsync("Hello, Menaver12345678901234567!");

            await Task.Delay(1000);

            await lcd.ClearAsync();

            await Task.Delay(1000);

            await lcd.WriteTextAsync("Hello, " + Environment.NewLine + "Menaver!");

            await Task.Delay(1000);

            await lcd.WriteTextAsync("Hello, " + Environment.NewLine + "Menaver!" + Environment.NewLine + "he");
        }
    }
}