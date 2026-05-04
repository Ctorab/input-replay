using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WindowsInput.Native;

namespace InputReplay
{
    public class KeyboardHook
    {
        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static event Action<VirtualKeyCode, bool>? KeyEvent;

        private static readonly ConcurrentQueue<(VirtualKeyCode, bool)> _queue = new();
        private static LowLevelProc? _proc;
        private static IntPtr _hookID = IntPtr.Zero;
        private static uint _hookThreadId;
        private static Thread? _hookThread;
        private static Thread? _workerThread;
        private static volatile bool _running;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        public static void Start()
        {
            _running = true;

            _workerThread = new Thread(ProcessQueue) { IsBackground = true };
            _workerThread.Start();

            _hookThread = new Thread(() =>
            {
                _hookThreadId = GetCurrentThreadId();

                _proc = HookCallback;
                using var process = Process.GetCurrentProcess();
                using var module = process.MainModule!;
                _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(module.ModuleName!), 0);

                while (GetMessage(out MSG msg, IntPtr.Zero, 0, 0) > 0)
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }

                UnhookWindowsHookEx(_hookID);
            })
            { IsBackground = true };

            _hookThread.Start();
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                bool isDown = wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN;
                _queue.Enqueue(((VirtualKeyCode)vkCode, isDown));
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void ProcessQueue()
        {
            while (_running)
            {
                while (_queue.TryDequeue(out var evt))
                    KeyEvent?.Invoke(evt.Item1, evt.Item2);
                Thread.Sleep(1);
            }
            while (_queue.TryDequeue(out var evt))
                KeyEvent?.Invoke(evt.Item1, evt.Item2);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int ptX, ptY;
        }

        [DllImport("user32.dll")] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")] private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("user32.dll")] private static extern bool TranslateMessage(ref MSG lpMsg);
        [DllImport("user32.dll")] private static extern IntPtr DispatchMessage(ref MSG lpMsg);
        [DllImport("user32.dll")] private static extern bool PostThreadMessage(uint idThread, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
        [DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string? lpModuleName);
    }
}