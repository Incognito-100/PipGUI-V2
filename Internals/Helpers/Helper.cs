using System.Diagnostics;
using System.Threading;

namespace PipGUI_V2.Internals.Helpers
{
    public class Helper
    {
        public static void CMD(string arg, int delay)
        {
            Process d = new
            Process();
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