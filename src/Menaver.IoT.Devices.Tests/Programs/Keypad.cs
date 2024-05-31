using Menaver.IoT.Devices.Keyboards;

namespace Menaver.IoT.Devices.Tests.Programs;

internal class Keypad
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

        while (true)
        {
            var key = await keypad4x4.ReadKeyAsync();

            Console.WriteLine($"Key pressed: {key}");
        }
    }
}