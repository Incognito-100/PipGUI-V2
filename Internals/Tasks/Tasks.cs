using PipGUI_V2.Internals.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace PipGUI_V2.Internals.Tasks
{
    public static class Tasks
    {
        public static async Task Getpac()
        {
            Vars.Packs.Clear();
            string temppath = Path.GetTempPath();
            Directory.SetCurrentDirectory(temppath);
            string reqfilepath = Path.Combine(temppath, "requirements.txt");

            // Delete requirements file if it already exists
            if (File.Exists(reqfilepath))
            {
                File.Delete(reqfilepath);
            }

            // Create requirements file and read its contents
            string makereqfile = "/C pip freeze > requirements.txt";
            Helper.Cmd(makereqfile, 500);

            while (true)
            {
                try
                {
                    StreamReader r = new(reqfilepath);
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        Vars.Packs.Add(line);
                    }
                    break; // break out of the while loop if file was read successfully
                }
                catch
                {
                    // if file not readable, wait for 5 seconds and try again
                    await Task.Delay(5000);
                }
            }
        }
    }
}