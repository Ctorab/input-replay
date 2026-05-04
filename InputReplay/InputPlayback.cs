using InputReplay.Model;
using WindowsInput;

namespace InputReplay
{
    internal class InputPlayback
    {
        private readonly InputSimulator _inputSimulator = new();

        public async Task Play(List<CapturedInput> inputs)
        {
            var sorted = inputs.OrderBy(x => x.tick).ToList();

            long start = sorted[0].tick;

            foreach (var input in sorted)
            {
                long delay = input.tick - start;
                start = input.tick;

                await Task.Delay((int)Math.Max(0, delay));

                if (input.isKeyDown)
                    _inputSimulator.Keyboard.KeyDown(input.input);
                else
                    _inputSimulator.Keyboard.KeyUp(input.input);

                _inputSimulator.Mouse.MoveMouseTo(input.x, input.y);

                if (!input.isKeyDown)
                    continue;

                if(input.input == WindowsInput.Native.VirtualKeyCode.LBUTTON)
                    _inputSimulator.Mouse.LeftButtonClick();
                else if(input.input == WindowsInput.Native.VirtualKeyCode.RBUTTON)
                    _inputSimulator.Mouse.RightButtonClick();
            }
        }
    }
}
