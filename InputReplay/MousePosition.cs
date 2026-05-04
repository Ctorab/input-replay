using System.Runtime.InteropServices;

namespace InputReplay
{
    public static class MousePosition
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        private static volatile int _x;
        private static volatile int _y;

        private static Thread? _thread;
        private static bool _running;

        public static void Start(int intervalMs = 10)
        {
            if (_running) return;

            _running = true;

            _thread = new Thread(() =>
            {
                while (_running)
                {
                    if (GetCursorPos(out POINT p))
                    {
                        _x = p.X;
                        _y = p.Y;
                    }

                    Thread.Sleep(intervalMs);
                }
            })
            { IsBackground = true };

            _thread.Start();
        }

        public static void Stop()
        {
            _running = false;
            _thread?.Join();
        }

        public static (int X, int Y) Get() => (_x, _y);
    }
}