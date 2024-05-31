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
- 🌡️ [Humidity & Temperature Sensor](https://www.mouser.com/datasheet/2/758/DHT11-Technical-Data-Sheet-Translated-Version-1143054.pdf) (HDT): HDT11, HDT12, HDT21, HDT22 

# Examples <a name="examples"></a>

More examples of using the IoT devices can be found in the test project [Menaver.IoT.Devices.Tests](https://github.com/Menaver/Menaver.IoT.Devices/tree/main/src/Menaver.IoT.Devices.Tests/Programs).

### Obtaining Humidity & Temperature data from HDT sensor

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
