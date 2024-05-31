using Iot.Device.DHTxx;

namespace Menaver.IoT.Devices.Sensors;

/// <summary>
///     Abstraction of interaction with Humidity & Temperature (HDT) sensor. Navigate to
///     <see href="https://www.mouser.com/datasheet/2/758/DHT11-Technical-Data-Sheet-Translated-Version-1143054.pdf">
///         Humidity & Temperature Sensor
///     </see>
///     for more details about the module.
/// </summary>
public interface IDht : IDisposable
{
    Task<double> GetTemperatureAsync(TemperatureUnit unit);
    Task<double> GetTemperatureAsync(TemperatureUnit unit, CancellationToken cancellationToken);
    Task<double> GetHumidityAsync();
    Task<double> GetHumidityAsync(CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of interaction with Humidity & Temperature (HDT) sensor. Navigate to
///     <see href="https://www.mouser.com/datasheet/2/758/DHT11-Technical-Data-Sheet-Translated-Version-1143054.pdf">
///         Humidity & Temperature Sensor
///     </see>
///     for more details about the module.
/// </summary>
public class Dht : IDht
{
    private readonly DhtBase _dht;

    public Dht(int dataPin, DhtDevice dhtDevice)
    {
        _dht = dhtDevice switch
        {
            DhtDevice.Dht11 => new Dht11(dataPin),
            DhtDevice.Dht12 => new Dht12(dataPin),
            DhtDevice.Dht21 => new Dht21(dataPin),
            DhtDevice.Dht22 => new Dht22(dataPin),
            _ => throw new ArgumentOutOfRangeException(nameof(dhtDevice), dhtDevice, null)
        };
    }

    public Task<double> GetTemperatureAsync(TemperatureUnit unit)
    {
        return GetTemperatureAsync(unit, CancellationToken.None);
    }

    public async Task<double> GetTemperatureAsync(TemperatureUnit unit, CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_dht.TryReadTemperature(out var temperature))
            {
                return unit switch
                {
                    TemperatureUnit.Celsius => temperature.DegreesCelsius,
                    TemperatureUnit.Delisle => temperature.DegreesDelisle,
                    TemperatureUnit.Fahrenheit => temperature.DegreesFahrenheit,
                    TemperatureUnit.Newton => temperature.DegreesNewton,
                    TemperatureUnit.Rankine => temperature.DegreesRankine,
                    TemperatureUnit.Reaumur => temperature.DegreesReaumur,
                    TemperatureUnit.Roemer => temperature.DegreesRoemer,
                    TemperatureUnit.Kelvin => temperature.Kelvins,
                    _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
                };
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
        }
    }

    public Task<double> GetHumidityAsync()
    {
        return GetHumidityAsync(CancellationToken.None);
    }

    public async Task<double> GetHumidityAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_dht.TryReadHumidity(out var humidity))
            {
                return humidity.Percent;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
        }
    }

    #region IDisposable

    ~Dht()
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
                _dht.Dispose();
            }

            _disposedValue = true;
        }
    }

    #endregion
}

public enum TemperatureUnit : byte
{
    Celsius = 0,
    Delisle = 1,
    Fahrenheit = 2,
    Newton = 3,
    Rankine = 4,
    Reaumur = 5,
    Roemer = 6,
    Kelvin = 7
}

public enum DhtDevice : byte
{
    Dht11 = 0,
    Dht12 = 1,
    Dht21 = 2,
    Dht22 = 3
}