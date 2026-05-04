using WindowsInput.Native;

namespace InputReplay.Model
{
    public record CapturedInput(VirtualKeyCode input, bool isKeyDown, double x, double y, long tick);
}
