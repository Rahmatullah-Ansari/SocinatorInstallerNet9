using SocinatorInstaller.Utilities;
using SocinatorInstaller.ViewModels;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace SocinatorInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow instance;
        public MainWindow()
        {
            InitializeComponent();
            if (IsAdministrator())
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Constants.GetInstallerExe,
                    Arguments = string.Empty,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception) { }
                finally
                {
                    Application.Current.Shutdown();
                    Environment.Exit(0);
                }
                return;
            }
            MainGrid.DataContext = MainViewModel.GetInstance;
            DirectoryUtility.CreateDirectory($"{Constants.InstallerFolder}");
            instance = this;
        }
        public static MainWindow GetInstance => instance;
        public void CloseWindow()
        {
            try
            {
                this.Close();
            }
            catch { }
        }
        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}