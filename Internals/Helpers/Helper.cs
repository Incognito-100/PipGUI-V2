using System.Diagnostics;
using System.Threading;

namespace PipGUI_V2.Internals.Helpers
{
    public static class Helper
    {
        public static void Cmd(string arg, int delay)
        {
            Process d = new Process();
            d.StartInfo.UseShellExecute = true;
            d.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            d.StartInfo.FileName = "CMD.exe";
            d.StartInfo.Arguments = arg;
            d.Start();
            d.Close();
            d.Dispose();
            Thread.Sleep(delay);
        }
    }
}