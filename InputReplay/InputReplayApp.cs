using WindowsInput.Native;

namespace InputReplay
{
    internal class InputReplayApp
    {
        private InputRecorder _inputRecorder = new();
        private InputPlayback _inputPlayback = new();

        public async Task Run(string[] args)
        {
            if(args.Length == 0)
            {
                LogHelp();
                return;
            }

            var command = args[0].ToLower();

            switch (command)
            {
                case "record":
                    await Record(args);
                    break;

                case "play":
                    await Play(args);
                    break;

                default:
                    LogHelp();
                    break;
            }
        }

        private async Task Record(string[] args)
        {
            string file = args.Length > 1 ? args[1] : "save.txt";

            Console.WriteLine("Press F6 to START, F7 to STOP");

            bool isRecording = false;

            while (true)
            {
                if (!isRecording && InputSimulatorHelper.IsKeyDown(VirtualKeyCode.F6))
                {
                    _inputRecorder.Start();
                    isRecording = true;
                    Console.WriteLine("Recording started...");
                }
                if (isRecording && InputSimulatorHelper.IsKeyDown(VirtualKeyCode.F7))
                {
                    var data = _inputRecorder.Stop();
                    InputFile.Save(file, data);

                    Console.WriteLine($"Saved {data.Count} events");
                    break;
                }

                await Task.Delay(5);
            }
        }

        private async Task Play(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing file path.");
                return;
            }

            string file = args[1];

            if (!File.Exists(file))
            {
                Console.WriteLine("File not found.");
                return;
            }

            var data = InputFile.Load(file);

            Console.WriteLine($"Playing {data.Count} keys...");
            await _inputPlayback.Play(data);
            Console.WriteLine("Done.");
        }

        private void LogHelp()
        {
            Console.WriteLine("""
        InputReplay usage:

        record <file>   - start recording input
        play <file>     - replay input file
        """);
        }
    }
}
