using WindowsInput;
using WindowsInput.Native;

namespace InputReplay
{
    internal static class InputSimulatorHelper
    {
        private static InputSimulator _inputSimulator = new();

        public static bool IsKeyDown(VirtualKeyCode virtualKeyCode) => _inputSimulator.InputDeviceState.IsKeyDown(virtualKeyCode);
    }
}
