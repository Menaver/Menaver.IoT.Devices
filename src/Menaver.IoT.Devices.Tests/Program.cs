using Menaver.IoT.Devices.Tests.Programs;

namespace Menaver.IoT.Devices.Tests;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Program name is missing in the args.");
            return 0;
        }

        switch (args[0].ToLower())
        {
            case "ledblinking": return await LedBlinking.RunAsync();
            case "lcdisplay": return await LcDisplay.RunAsync();
            case "lcdkeypad": return await LcdKeypad.RunAsync();
            case "keypad": return await Keypad.RunAsync();
            case "thermo": return await Thermometer.RunAsync();
            default:
            {
                Console.WriteLine("Program name is not supported.");
                return 0;
            }
        }
    }
}