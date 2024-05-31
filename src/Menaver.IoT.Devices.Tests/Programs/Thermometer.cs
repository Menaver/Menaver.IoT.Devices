using Menaver.IoT.Devices.Sensors;

namespace Menaver.IoT.Devices.Tests.Programs;

internal class Thermometer
{
    public static async Task<int> RunAsync()
    {
        using var dht = new Dht(17, DhtDevice.Dht11);

        Console.WriteLine("Waiting for the sensor to response...");

        while (true)
        {
            var temperature = await dht.GetTemperatureAsync(TemperatureUnit.Celsius, CancellationToken.None);
            Console.WriteLine($"Temperature: {temperature:F1}\u00B0C");

            var humidity = await dht.GetHumidityAsync(CancellationToken.None);
            Console.WriteLine($"Humidity: {humidity:F1}%");

            await Task.Delay(1000);
        }
    }
}