using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.CharacterLcd;
using Iot.Device.Pcx857x;

namespace Menaver.IoT.Devices.Displays;

/// <summary>
///     Abstraction of interaction with LCD display module (PCX857X).
///     Navigate to
///     <see href="https://github.com/nanoframework/nanoframework.IoT.Device/blob/develop/devices/Pcx857x/README.md/">PCX857X</see>
///     for more details about the module.
/// </summary>
public interface ILcdPcx857x
{
    Task WriteTextAsync(string text);
    Task WriteTextAsync(string text, CancellationToken cancellationToken);
    Task ClearAsync();
    Task ClearAsync(CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of interaction with LCD display module (PCX857X).
///     Navigate to
///     <see href="https://github.com/nanoframework/nanoframework.IoT.Device/blob/develop/devices/Pcx857x/README.md/">PCX857X</see>
///     for more details about the module.
/// </summary>
public class LcdPcx857x : ILcdPcx857x
{
    // For PCF8574T i2c addresses can be between 0x27 and 0x20 depending on bridged solder jumpers
    // and for PCF8574AT i2c addresses can be between 0x3f and 0x38 depending on bridged solder jumpers
    private static readonly int[] PossiblePorts = { 0x3F, 0x3E, 0x3D, 0x3C, 0x3B, 0x3A, 0x39, 0x38, 0x27 };
    private readonly int[] _dataPins;

    private readonly Pcx857xDevice _device;
    private readonly int _deviceActualPortAddress;
    private readonly int _displayColumnCount;
    private readonly int _displayRowCount;
    private readonly int _sclPin;
    private readonly int _sdaPin;

    public LcdPcx857x(
        Pcx857xDevice device,
        int sdaPin,
        int sclPin,
        int displayRowCount,
        int displayColumnCount,
        int[] dataPins = null!)
    {
        _device = device;

        foreach (var possiblePort in PossiblePorts)
        {
            if (CheckDevice(_device, possiblePort, sdaPin, sclPin, dataPins))
            {
                _deviceActualPortAddress = possiblePort;
                _sdaPin = sdaPin;
                _sclPin = sclPin;
                _dataPins = dataPins;
                _displayRowCount = displayRowCount;
                _displayColumnCount = displayColumnCount;

                break;
            }

            throw new ArgumentException(
                "LCD PCF8574 isn't accessible. Please, make sure device is injected and enabled.");
        }
    }

    public LcdPcx857x(
        Pcx857xDevice device,
        int portAddressAddress,
        int sdaPin,
        int sclPin,
        int displayRowCount,
        int displayColumnCount,
        int[] dataPins = null!)
    {
        _device = device;

        if (!CheckDevice(_device, portAddressAddress, sdaPin, sclPin, dataPins))
        {
            throw new ArgumentException(
                "LCD PCF8574 isn't accessible by the port and pins specified. Please, try another ones.");
        }

        _deviceActualPortAddress = portAddressAddress;
        _sdaPin = sdaPin;
        _sclPin = sclPin;
        _dataPins = dataPins;
        _displayRowCount = displayRowCount;
        _displayColumnCount = displayColumnCount;
    }

    public Task WriteTextAsync(string text)
    {
        return WriteTextAsync(text, CancellationToken.None);
    }

    public Task WriteTextAsync(string text, CancellationToken cancellationToken)
    {
        var task = new Task(() => WriteText(text));

        task.Start();
        Task.WaitAll(new[] { task }, cancellationToken);

        return task;
    }

    public Task ClearAsync()
    {
        return ClearAsync(CancellationToken.None);
    }

    public Task ClearAsync(CancellationToken cancellationToken)
    {
        var task = new Task(ClearScreen);

        task.Start();
        Task.WaitAll(new[] { task }, cancellationToken);

        return task;
    }

    private void WriteText(string text)
    {
        var connection = new I2cConnectionSettings(busId: 1, deviceAddress: _deviceActualPortAddress);
        using var module = I2cDevice.Create(connection);
        using var driver = BuildDriver(_device, module);
        using var controller = new GpioController(PinNumberingScheme.Logical, driver);

        using var lcd = new Lcd1602(
            registerSelectPin: 0,
            enablePin: _sdaPin,
            backlightPin: _sclPin,
            dataPins: _dataPins ?? new[] { 4, 5, 6, 7 },
            controller: controller);

        lcd.Clear();

        string[] lines;
        if (text.Contains(Environment.NewLine))
        {
            lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }
        else
        {
            if (text.Length <= _displayColumnCount)
            {
                lines = new[] { text };
            }
            else
            {
                // split by length to fix the display size
                lines = new string[_displayRowCount];
                for (var i = 0; i < _displayRowCount; i++)
                {
                    if (_displayColumnCount * i + _displayColumnCount >= text.Length)
                    {
                        lines[i] = text.Substring(_displayColumnCount * i, text.Length - _displayColumnCount);
                    }
                    else
                    {
                        lines[i] = text.Substring(_displayColumnCount * i, _displayColumnCount);
                    }
                }
            }
        }

        for (var i = 0; i < lines.Length; i++)
        {
            if (i == _displayRowCount)
            {
                // to prevent falling out of bound
                return;
            }

            try
            {
                lcd.SetCursorPosition(0, i);
                lcd.Write(lines[i]);
            }
            catch (ArgumentOutOfRangeException)
            {
                // this occurs when we're out of bound
                break;
            }
        }
    }

    private void ClearScreen()
    {
        var connection = new I2cConnectionSettings(busId: 1, deviceAddress: _deviceActualPortAddress);
        using var module = I2cDevice.Create(connection);
        using var driver = BuildDriver(_device, module);
        using var controller = new GpioController(PinNumberingScheme.Logical, driver);

        using var lcd = new Lcd1602(
            registerSelectPin: 0,
            enablePin: _sdaPin,
            backlightPin: _sclPin,
            dataPins: _dataPins ?? new[] { 4, 5, 6, 7 },
            controller: controller);

        lcd.Clear();
    }

    private static bool CheckDevice(Pcx857xDevice device, int portAddress, int sdaPin, int sclPin, int[] dataPins)
    {
        try
        {
            var connection = new I2cConnectionSettings(busId: 1, deviceAddress: portAddress);
            using var module = I2cDevice.Create(connection);
            using var driver = BuildDriver(device, module);
            using var controller = new GpioController(PinNumberingScheme.Logical, driver);

            using var lcd = new Lcd1602(
                registerSelectPin: 0,
                enablePin: sdaPin,
                backlightPin: sclPin,
                dataPins: dataPins ?? new[] { 4, 5, 6, 7 },
                controller: controller);

            lcd.Clear();
            lcd.SetCursorPosition(0, 0);
            lcd.Write("test");
            lcd.Clear();

            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static Pcx857x BuildDriver(Pcx857xDevice device, I2cDevice module)
    {
        return device switch
        {
            Pcx857xDevice.Pcf8574 => new Pcf8574(module),
            Pcx857xDevice.Pcf8575 => new Pcf8575(module),
            Pcx857xDevice.Pca8574 => new Pca8574(module),
            Pcx857xDevice.Pca8575 => new Pca8575(module),
            _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
        };
    }
}

public enum Pcx857xDevice : byte
{
    Pcf8574 = 0,
    Pcf8575 = 1,
    Pca8574 = 2,
    Pca8575 = 3
}