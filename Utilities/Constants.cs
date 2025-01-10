using SocinatorInstaller.Enums;
using System;

namespace SocinatorInstaller.Utilities
{
    public class Constants
    {
        #region API Server Links
        public static double UIOpacityEnable = 1;
        public static double UIOpacityDisable = 0.6;
        public static bool DeveloperMode = true;
        public static string IconFileName => "SocinatorSmall.ico";
        public static string ShortCutDescription => "Social Dominator";
        public static string GetLocalFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string GetDownloadNode(ConfigMode configMode) => $"{ApplicationName}\\{configMode}";
        public static string ApplicationName { get; set; } = "Socinator";
        public static string AssemblyName => System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
        public static bool IsCloudBucket { get; set; } = false;
        public static string InstallerFolder => $"{GetLocalFolder}\\{ApplicationName}Installer";
        public static string ConfirmationMessageForClosing { get; set; } = $"{ApplicationName} is running Do you want to close before uninstalling ?";
        public static string GetDefaultIntallationPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string GetInstallerExe => (System.Reflection.Assembly.GetEntryAssembly().Location)?.Replace(".dll",".exe");
        //For DevelopeMentAPI
        public static Uri uri { get; set; } = new Uri(@"https://storage.googleapis.com/powerbrowser-bulids/Power-dev/power-dev-bulids/Power%20Browser%20Dev%20Installer.exe");
        //For LiveAPI
        //  public static Uri uri = new Uri(@"https://storage.googleapis.com/powerbrowser-bulids/Power-live/power-live-builds/Power%20Browser%20Installer.exe");
        #endregion
    }
}
