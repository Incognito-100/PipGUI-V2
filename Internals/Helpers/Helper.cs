using System.Diagnostics;
using System.Threading;

namespace PipGUI_V2.Internals.Helpers
{
    public static class Helper
    {
        public static void Cmd(string command, int delay)
        {
            Process process = new();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "CMD.exe";
            process.StartInfo.Arguments = command;
            process.Start();
            process.Close();

            Thread.Sleep(delay);
        }
    }
}