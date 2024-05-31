using System.Device.Gpio;
using System.Timers;
using Iot.Device.KeyMatrix;
using Timer = System.Timers.Timer;

namespace Menaver.IoT.Devices.Keyboards;

/// <summary>
///     Abstraction of interaction with matrix keyboard circuit.
///     Navigate to <see href="https://en.wikipedia.org/wiki/Keyboard_matrix_circuit">Keyboard matrix circuit</see>
///     for more details about the module.
/// </summary>
public interface IMatrixKeyboard : IDisposable
{
    Task<char> ReadKeyAsync();
    Task<char> ReadKeyAsync(CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of interaction with matrix keyboard circuit.
///     Navigate to <see href="https://en.wikipedia.org/wiki/Keyboard_matrix_circuit">Keyboard matrix circuit</see>
///     for more details about the module.
/// </summary>
public class MatrixKeyboard : IMatrixKeyboard
{
    private readonly char[,] _keyMap;
    private readonly KeyMatrix _keypad;
    private readonly Timer _pressListeningTimer;

    private CancellationTokenSource _pressListeningCancellationTokenSource;

    public MatrixKeyboard(
        int[] inputPins,
        int[] outputPins,
        char[,] keyMap,
        int scanIntervalInMilliseconds = 10)
    {
        if (keyMap.Rank != 2)
        {
            throw new ArgumentException("The key map is supposed to be 2-dimensional array.");
        }

        if (keyMap.GetLength(0) != inputPins.Length)
        {
            throw new ArgumentException("Input pin count does not match the keymap row count.");
        }

        if (keyMap.GetLength(1) != outputPins.Length)
        {
            throw new ArgumentException("Output pin count does not match the keymap column count.");
        }

        _keyMap = keyMap;

        _keypad = new KeyMatrix(outputPins, inputPins, TimeSpan.FromMilliseconds(scanIntervalInMilliseconds));

        _pressListeningTimer = new Timer(scanIntervalInMilliseconds * (outputPins.Length + 3));
        _pressListeningTimer.Elapsed += PressListeningTimerOnElapsed;
    }

    public async Task<char> ReadKeyAsync()
    {
        return await ReadKeyAsync(CancellationToken.None);
    }

    public async Task<char> ReadKeyAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var key = await ReadKeyRaisedAsync(cancellationToken);

            if (key != null)
            {
                _pressListeningCancellationTokenSource = new CancellationTokenSource();
                _pressListeningTimer.Enabled = true;

                var rowIndex = key.Input;
                var columnIndex = await DetermineColumnIndexAsync(key.Output,
                    _pressListeningCancellationTokenSource.Token);

                if (columnIndex != null)
                {
                    return _keyMap[rowIndex, columnIndex.Value];
                }
            }
        }
    }

    private async Task<int?> DetermineColumnIndexAsync(int columnIndexToExclude, CancellationToken cancellationToken)
    {
        var columnsCount = _keyMap.GetLength(1);
        var columns = new List<int>(columnsCount - 1);
        for (var i = 0; i < columnsCount; i++)
        {
            if (i == columnIndexToExclude)
            {
                continue; // exclude the column that's raised above 
            }

            columns.Add(i);
        }

        do
        {
            try
            {
                var key = await ReadKeyRaisedAsync(cancellationToken);

                if (key != null)
                {
                    var columnRaised = key.Output;
                    columns.Remove(columnRaised); // exclude the column that was raised
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        } while (columns.Any());

        // return the one that's left (remained not triggered, i.g. pressed)
        return columns.Any() ? columns.First() : null;
    }

    private async Task<KeyMatrixEvent?> ReadKeyRaisedAsync(CancellationToken cancellationToken)
    {
        var task = new Task<KeyMatrixEvent>(() => _keypad.ReadKey());

        task.Start();
        Task.WaitAll(new Task[] { task }, cancellationToken);

        var key = await task;

        return key?.EventType.HasFlag(PinEventTypes.Rising) == true ? key : null;
    }

    private void PressListeningTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _pressListeningTimer.Enabled = false;
        _pressListeningCancellationTokenSource.Cancel();
    }

    #region IDisposable

    ~MatrixKeyboard()
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
                _keypad.StopListeningKeyEvent();
                _keypad.Dispose();
            }

            _disposedValue = true;
        }
    }

    #endregion
}