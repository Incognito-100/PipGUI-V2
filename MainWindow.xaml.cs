using Microsoft.VisualBasic;
using Microsoft.Win32;
using PipGUI_V2.Internals;
using PipGUI_V2.Internals.Helpers;
using PipGUI_V2.Internals.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Path = System.IO.Path;

namespace PipGUI_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //==========================================|get installed packages|==========================================
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            paclist.Items.Clear();
            await Task.Run(() => Tasks.Getpac());
            foreach (string pakc in Vars.packs)
            {
                paclist.Items.Add(pakc);
            }
            Vars.packs.Clear();
        }

        //==========================================|install packages|==========================================
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        restoret:
            string instpacname = Interaction.InputBox("Enter name of package you want to install", "install", "");

            //==========================================|no package to install|==========================================
            if (string.IsNullOrEmpty(instpacname))
            {
                MessageBox.Show("no name given",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
            //==========================================|install package|==========================================
            else
            {
                string api = $"https://pypi.org/pypi/{instpacname}/json";
                var client = new HttpClient();

                try
                {
                    var res = client.GetStringAsync(api).Result;
                }
                catch
                {
                    MessageBox.Show("requested package doesn't exist or website is down try again",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                    goto restoret;
                }

                var inpres = MessageBox.Show($"do you want to install {instpacname}?", "confirm", MessageBoxButton.YesNo);

                switch (inpres)
                {
                    //==========================================|yes|==========================================
                    case MessageBoxResult.Yes:
                        string command = $"/C pip install {instpacname}";
                        Process.Start("CMD.exe", command);
                        break;

                    //==========================================|no|==========================================
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        //==========================================|unininstall a package|==========================================
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (paclist.Items.Count != 0)
            {
                string s = "";
                foreach (object p in paclist.SelectedItems)
                {
                    s = $"{s}{p}\n";
                }

                string temppath = Path.GetTempPath();
                Directory.SetCurrentDirectory(temppath);

                if (File.Exists($@"{temppath}\packslist.txt"))
                {
                    File.Delete($@"{temppath}\packslist.txt");

                    File.WriteAllText("packslist.txt", s);
                    string uninststeps = "/C pip uninstall -r packslist.txt -y";

                    Process.Start("CMD.exe", uninststeps);
                }

                //==========================================|make req file and read|==========================================
                else
                {
                    File.WriteAllText("packslist.txt", s);
                    string uninststeps = "/C pip uninstall -r packslist.txt -y";

                    Process.Start("CMD.exe", uninststeps);
                }
            }
            //==========================================|no package to uninstall|==========================================
            else
            {
                MessageBox.Show("no package selected",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        //==========================================|installs a requirements file|==========================================
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "install a requirements file";
            ofd.DefaultExt = "txt";
            ofd.Filter = "Text Files|*.txt";
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            if (ofd.ShowDialog() == true)
            {
                string line = ofd.FileName;

                string Command = $"/C pip install -r {line}";

                Process.Start("CMD.exe", Command);
            }
        }

        //==========================================|uninstall a requirements file|==========================================
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "uninstall a requirements file";
            ofd.DefaultExt = "txt";
            ofd.Filter = "Text Files|*.txt";
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            if (ofd.ShowDialog() == true)
            {
                string line = ofd.FileName;

                string Command = $"/C pip uninstall -r {line}";

                Process.Start("CMD.exe", Command);
            }
        }

        //==========================================|generate a requirements file|==========================================
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "open python file";
            ofd.DefaultExt = "py";
            ofd.Filter = "python files|*.py";
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            if (ofd.ShowDialog() == true)
            {
                //==========================================|setup lists|==========================================
                List<string> imports = new List<string>();
                List<string> cleand = new List<string>();

                string[] lines = File.ReadAllLines(ofd.FileName);
                foreach (string line in lines)
                {
                    if (line.Contains("import"))
                    {
                        imports.Add(line);
                    }
                }
                //==========================================|clean imports|==========================================
                foreach (string line in imports)
                {
                    string clean = line.Replace("from ", "").Replace("import ", "").Replace(" ", "");
                    cleand.Add(clean);
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "save txt file";
                sfd.DefaultExt = ".txt";
                sfd.InitialDirectory = Directory.GetCurrentDirectory();
                sfd.Filter = "Text Files|*.txt";
                if (sfd.ShowDialog() == true)
                {
                    File.WriteAllLines(sfd.FileName, cleand);
                }
            }
        }

        //==========================================|uninstall all  requirements|==========================================
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var conf = MessageBox.Show("are you sure you want to uninstall all installed packages ?", "confirm",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

            switch (conf)
            {
                //==========================================|yes|==========================================
                case MessageBoxResult.Yes:

                    string temppath = Path.GetTempPath();
                    Directory.SetCurrentDirectory(temppath);

                    string makereqfile = "/C pip freeze > uninst.txt";

                    Helper.CMD(makereqfile, 500);

                    string uninstevery = "/C pip uninstall -r uninst.txt -y";
                    Process.Start("CMD.exe", uninstevery);

                    string clearcache = "/C pip cache purge";

                    Helper.CMD(clearcache, 500);
                    paclist.Items.Clear();
                    break;

                //==========================================|no|==========================================
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string temppath = Path.GetTempPath();
            Directory.SetCurrentDirectory(temppath);

            string[] files = { "packslist.txt", "requirements.txt", "uninst.txt" };

            //==========================================|check if dirs exist|==========================================
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                //==========================================|exti app after done deleting|==========================================
                finally
                {
                    App.Current.Shutdown();
                }
            }
        }
    }
}