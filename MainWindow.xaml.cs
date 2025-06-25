using Microsoft.VisualBasic;
using Microsoft.Win32;
using PipGUI_V2.Internals;
using PipGUI_V2.Internals.Helpers;
using PipGUI_V2.Internals.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
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
            try
            {
                // Clear the package list
                Paclist.Items.Clear();

                // Set the paclist to indicate that packages are being retrieved
                Paclist.Items.Add("Getting packages...");

                // Asynchronously get the list of packages
                await Task.Run(Tasks.Getpac);

                //clear paclist
                Paclist.Items.Clear();

                if (Vars.Packs.Count == 0)
                {
                    Paclist.Items.Add("No packages installed.");
                }
                else
                {
                    // Populate the package list with the retrieved packages
                    foreach (string pack in Vars.Packs)
                    {
                        Paclist.Items.Add(pack);
                    }

                    // Clear the list of packages
                    Vars.Packs.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        //==========================================|Install packages|==========================================
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
            //==========================================|Install package|==========================================
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
                    //==========================================|ydes|==========================================
                    case MessageBoxResult.Yes:
                        string command = $"/C pip install {instpacname}";
                        Process.Start("CMD.exe", command);
                        break;

                    //==========================================|No|==========================================
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        //==========================================|Unininstall a package|==========================================
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Paclist.Items.Count == 0)
            {
                MessageBox.Show("No package selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedPackages = "";
            foreach (var package in Paclist.SelectedItems)
            {
                selectedPackages += $"{package}\n";
            }

            string tempPath = Path.GetTempPath();
            Directory.SetCurrentDirectory(tempPath);

            string packListPath = Path.Combine(tempPath, "packslist.txt");
            File.WriteAllText(packListPath, selectedPackages);

            const string uninstallCommand = "/C pip uninstall -r packslist.txt -y";
            Process.Start("CMD.exe", uninstallCommand);
        }

        //==========================================|Installs a requirements file|==========================================
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "install a requirements file",
                DefaultExt = "txt",
                Filter = "Text Files|*.txt",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (ofd.ShowDialog() == true)
            {
                string line = ofd.FileName;

                string Command = $"/C pip install -r {line}";

                Process.Start("CMD.exe", Command);
            }
        }

        //==========================================|Uninstall a requirements file|==========================================
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Uninstall a requirements file",
                DefaultExt = "txt",
                Filter = "Text Files|*.txt",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                string command = $"/C pip uninstall -r {fileName}";

                Process.Start("CMD.exe", command);
            }
        }

        //==========================================|Generate a requirements file|==========================================
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            // Create open file dialog
            var openDialog = new OpenFileDialog
            {
                Title = "Open Python file",
                DefaultExt = ".py",
                Filter = "Python files (*.py)|*.py",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            // Show open file dialog and process the file
            if (openDialog.ShowDialog() == true)
            {
                // Read the lines from the selected file
                var lines = File.ReadAllLines(openDialog.FileName);

                // Extract the import statements from the file
                var imports = lines.Where(l => l.TrimStart().StartsWith("import")).ToList();

                // Clean the import statements by removing "from" and "import" keywords
                var cleanedImports = imports.Select(i => i.Replace("from ", "").Replace("import ", "").Replace(" ", ""));

                // Create save file dialog
                var saveDialog = new SaveFileDialog
                {
                    Title = "save txt file",
                    DefaultExt = ".txt",
                    InitialDirectory = Directory.GetCurrentDirectory(),
                    Filter = "Text Files|*.txt"
                };
                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllLines(saveDialog.FileName, cleanedImports);
                }
            }
        }

        //==========================================|uninstall all requirements|==========================================
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var conf = MessageBox.Show("Are you sure you want to uninstall all installed packages?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (conf)
            {
                //==========================================|yes|==========================================
                case MessageBoxResult.Yes:
                    string temppath = Path.GetTempPath();
                    Directory.SetCurrentDirectory(temppath);

                    const string makereqfile = "/C pip freeze > uninst.txt";

                    Helper.Cmd(makereqfile, 500);

                    const string uninstevery = "/C pip uninstall -r uninst.txt -y";
                    Process.Start("CMD.exe", uninstevery);

                    const string clearcache = "/C pip cache purge";

                    Helper.Cmd(clearcache, 500);

                    Paclist.Items.Clear();
                    break;

                //==========================================|no|==========================================
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string temppath = Path.GetTempPath();
            Directory.SetCurrentDirectory(temppath);

            string[] files = ["packslist.txt", "requirements.txt", "uninst.txt"];

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
                finally
                {
                    // Exit the application after deleting the files
                    Application.Current.Shutdown();
                }
            }
        }
    }
}