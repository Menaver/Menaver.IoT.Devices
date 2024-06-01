# Overview <a name="overview"></a>

.NET implementations for various IoT devices (LED, displays, keyboards, sensors, etc).

## Keynotes
- Platform: netstandard2.0;
- Distributed via NuGet (check out [Menaver.IoT.Devices](https://www.nuget.org/packages/Menaver.IoT.Devices))
- This repository [leverages](https://github.com/Menaver/menaver.iot.devices/actions) Github Actions (GHA);

## Supported devices
- 💡 [Light-emitting diode](https://en.wikipedia.org/wiki/Light-emitting_diode) (LED)
- ⌨️ [Matrix Keyboard M×N](https://en.wikipedia.org/wiki/Keyboard_matrix_circuit) 
- 📟 [Liquid-crystal display](https://en.wikipedia.org/wiki/Liquid-crystal_display) (LCD): PCF8574, PCF8575, PCA8574, PCA8575 
- 🌡️ [Humidity & Temperature Sensor](https://www.mouser.com/datasheet/2/758/DHT11-Technical-Data-Sheet-Translated-Version-1143054.pdf) (DHT): DHT11, DHT12, DHT21, DHT22 

# Examples <a name="examples"></a>

More examples of using the IoT devices can be found in the test project [Menaver.IoT.Devices.Tests](https://github.com/Menaver/Menaver.IoT.Devices/tree/main/src/Menaver.IoT.Devices.Tests/Programs).

## Basic LED

#### Scheme

<img src="https://github.com/Menaver/Menaver.IoT.Devices/assets/12029392/b9fb32cd-c463-416d-9a9c-fa478ae991c9" width="480">

#### Code
```cs
List<Led> leds = new()
{
    new Led(4, Color.Red, false),
};

using var ledPanel = new LedPanel(leds);

ledPanel.SetAll(Color.Red);
await Task.Delay(300);

ledPanel.ResetAll();
await Task.Delay(300);

ledPanel.Toggle(4);
```

## Obtaining Humidity & Temperature data from HDT sensor

#### Scheme

<img src="https://github.com/Menaver/Menaver.IoT.Devices/assets/12029392/a48ca152-fc2d-48aa-86bc-21ef291892e0" width="480">

#### Code
```cs
using var dht = new Dht(dataPin: 4, DhtDevice.Dht22);

Console.WriteLine("Waiting for the sensor to response...");

while (true)
{
    var temperature = await dht.GetTemperatureAsync(TemperatureUnit.Celsius, CancellationToken.None);
    Console.WriteLine($"Temperature: {temperature:F1}\u00B0C");

    var humidity = await dht.GetHumidityAsync(CancellationToken.None);
    Console.WriteLine($"Humidity: {humidity:F1}%");

    await Task.Delay(1000);
}
```

## Matrix Keyboard 4x4

#### Scheme

<img src="https://github.com/Menaver/Menaver.IoT.Devices/assets/12029392/87dc0a3f-4be9-48f5-8357-aa5fd436e628" width="480">

#### Code
```cs
    private static readonly int[] _inputs = { 18, 23, 24, 25 };
    private static readonly int[] _outputs = { 10, 22, 27, 17 };

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
```
