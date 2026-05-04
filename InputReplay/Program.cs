using InputReplay;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("shcore.dll")]
    private static extern int SetProcessDpiAwareness(int awareness);    

    private static async Task Main(string[] args)
    {
        try
        {
            SetProcessDpiAwareness(2);
        }
        catch { }
        var app = new InputReplayApp();
        await app.Run(args);
    }
}