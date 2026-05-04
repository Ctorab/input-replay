using InputReplay.Model;
using System.Text;
using WindowsInput.Native;

namespace InputReplay
{
    public static class InputFile
    {
        public static void Save(string path, List<CapturedInput> inputs)
        {
            var sb = new StringBuilder();

            foreach (var i in inputs)
                sb.AppendLine($"{i.input}|{i.isKeyDown}|{i.x}|{i.y}|{i.tick}");

            File.WriteAllText(path, sb.ToString());
        }

        public static List<CapturedInput> Load(string path)
        {
            var lines = File.ReadAllLines(path);
            var result = new List<CapturedInput>();

            foreach (var line in lines)
            {
                var p = line.Split('|');

                result.Add(new CapturedInput(
                    Enum.Parse<VirtualKeyCode>(p[0]),
                    bool.Parse(p[1]),
                    double.Parse(p[2]),
                    double.Parse(p[3]),
                    long.Parse(p[4])
                ));
            }

            return result;
        }
    }
}
