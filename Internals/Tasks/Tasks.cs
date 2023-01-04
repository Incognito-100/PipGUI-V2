using PipGUI_V2.Internals.Helpers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PipGUI_V2.Internals.Tasks
{
    public static class Tasks
    {
        public static Task Getpac()
        {
            Vars.Packs.Clear();
            string temppath = Path.GetTempPath();
            Directory.SetCurrentDirectory(temppath);

            //==========================================|if req file alredy exists|==========================================
            if (File.Exists($@"{temppath}\requirements.txt"))
            {
                File.Delete($@"{temppath}\requirements.txt");

                string makereqfile = "/C pip freeze > requirements.txt";

                Helper.Cmd(makereqfile, 500);
            }
            //==========================================|make req file and read|==========================================
            else
            {
                string makereqfile = "/C pip freeze > requirements.txt";

                Helper.Cmd(makereqfile, 500);
            }

        //==========================================|get file dir|==========================================
        retryload:
            try
            {
                StreamReader r = new StreamReader($@"{temppath}\requirements.txt");

                string line;
                while ((line = r.ReadLine()) != null)
                {
                    Vars.Packs.Add(line);
                }
            }
            //==========================================|if file not readable|==========================================
            catch
            {
                Thread.Sleep(5000);
                goto retryload;
            }

            return Task.CompletedTask;
        }
    }
}