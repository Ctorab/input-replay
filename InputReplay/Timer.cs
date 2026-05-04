using System.Diagnostics;

namespace InputReplay
{
    public class Timer
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();

        public long GetTick() => _sw.ElapsedMilliseconds;
    }
}
