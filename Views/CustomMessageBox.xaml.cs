using SocinatorInstaller.Utilities;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SocinatorInstaller.Views
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : UserControl
    {
        public Window MyWindow { get; set; }
        private string dailoguemsg = string.Empty;
        public bool IsOk { get; set; }
        public CustomMessageBox(bool isYes = false, string msg = "")
        {
            dailoguemsg = msg;
            InitializeComponent();
            if (isYes)
            {
                btnNo.Visibility = Visibility.Collapsed;
                btnYes.Visibility = Visibility.Collapsed;
                btnOk.Visibility = Visibility.Visible;
            }
            if (!string.IsNullOrEmpty(msg))
            {
                txt_Message.Text = msg;
            }
        }

        private void clk_Yes(object sender, RoutedEventArgs e)
        {
            if (dailoguemsg == Constants.ConfirmationMessageForClosing)
            {
                IsOk = true;
                var dd = Process.GetProcessesByName(Constants.ApplicationName);
                foreach (var process in dd)
                {
                    try
                    {
                        // Check if the process name matches exactly
                        if (process.ProcessName.Equals(Constants.ApplicationName, StringComparison.OrdinalIgnoreCase))
                        {
                            // If the process did not exit, kill it
                            if (!process.HasExited)
                            {
                                process.Kill();
                                process.WaitForExit(); // Wait for the process to terminate
                            }
                            break; // Break the loop once the process is handled
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                MyWindow.Close();
            }
            else
            {
                IsOk = false;
                MyWindow.Close();
                Application.Current.Shutdown();
            }

        }

        private void clk_No(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            MyWindow.Close();
        }
    }
}
