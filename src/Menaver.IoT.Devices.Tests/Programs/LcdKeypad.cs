using Menaver.IoT.Devices.Displays;
using Menaver.IoT.Devices.Keyboards;

namespace Menaver.IoT.Devices.Tests.Programs;

internal class LcdKeypad
{
    private static readonly int[] _inputs = { 18, 23, 24, 25 };
    private static readonly int[] _outputs = { 6, 13, 19, 26 };

    private static readonly char[,] _keyMap =
    {
        { '1', '2', '3', 'A' },
        { '4', '5', '6', 'B' },
        { '7', '8', '9', 'C' },
        { '*', '0', '#', 'D' }
    };

    public static async Task<int> RunAsync()
    {
        using var keypad4x4 = new MatrixKeyboard(_inputs, _outputs, _keyMap);

        var lcd = new LcdPcx857x(
            Pcx857xDevice.Pcf8574,
            sdaPin: 2,
            sclPin: 3,
            displayRowCount: 2,
            displayColumnCount: 16);

        var text = string.Empty;
        while (true)
        {
            var key = await keypad4x4.ReadKeyAsync();

            text += key;

            await lcd.WriteTextAsync(text);
        }
    }
}