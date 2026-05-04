using InputReplay.Model;
using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;

namespace InputReplay;

public class InputRecorder
{
    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;

    private readonly InputSimulator _inputSimulator = new();
    private readonly Timer _timer = new();

    private readonly List<CapturedInput> _capturedInputs = new();

    private CancellationTokenSource? _cts;

    public void Start()
    {
        MousePosition.Start(5);
        KeyboardHook.Start();
        KeyboardHook.KeyEvent += RecordKeyStateChange;
        _cts = new CancellationTokenSource();
        Task.Run(() => CaptureLoop(_cts.Token));
    }

    private void RecordKeyStateChange(VirtualKeyCode key, bool isKeyDown)
    {
        var now = _timer.GetTick();
        var (x, y) = GetGlobalMousePosition();
        _capturedInputs.Add(new CapturedInput(key, isKeyDown, x, y, now));
    }

    public List<CapturedInput> Stop()
    {
        _cts?.Cancel();
        return _capturedInputs.ToList();
    }

    private void CaptureLoop(CancellationToken tkn)
    {
        var keys = Enum.GetValues<VirtualKeyCode>();

        while (!tkn.IsCancellationRequested)
        {
            var now = _timer.GetTick();
            foreach (var key in keys)
            {
                bool isDown = _inputSimulator.InputDeviceState.IsKeyDown(key);

                if (!(key == VirtualKeyCode.LBUTTON || key == VirtualKeyCode.RBUTTON))
                    continue;

                var (x, y) = GetGlobalMousePosition();
                _capturedInputs.Add(new CapturedInput(key, isDown, x, y, now));
            }

            Thread.Sleep(1);
        }
    }

    private (double X, double Y) GetGlobalMousePosition()
    {
        var (x, y) = MousePosition.Get();
        return GetGlobalMousePosition(x, y);
    }

    private (double X, double Y) GetGlobalMousePosition(int x, int y)
    {
        var screenWidth = GetSystemMetrics(SM_CXSCREEN);
        var screenHeight = GetSystemMetrics(SM_CYSCREEN);

        var absoluteX = x * 65535.0 / screenWidth;
        var absoluteY = y * 65535.0 / screenHeight;

        return (absoluteX, absoluteY);
    }
}