//using IWshRuntimeLibrary;
using Microsoft.Win32;
using SocinatorInstaller.Enums;
using SocinatorInstaller.Models;
using SocinatorInstaller.Utilities;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsShortcutFactory;

namespace SocinatorInstaller.Views
{
    /// <summary>
    /// Interaction logic for ReleaseUI.xaml
    /// </summary>
    public partial class ReleaseUI : UserControl, INotifyPropertyChanged
    {
        private static ReleaseUI instance;
        private FireBaseHelper helper;
        private SocialDominatorModel latestUploadedModel;
        public ReleaseUI()
        {
            InitializeComponent();
            helper = FireBaseHelper.GetInstance;
            gradientBrush.GradientStops.Add(new GradientStop(color1, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(color2, 1.0));
            // Apply the brush to the Rectangle's Fill

            gradientRectangle1.Fill = gradientBrush;
            gradientRectangle2.Fill = gradientBrush;
            gradientRectangle3.Fill = gradientBrush;
            gradientRectangle4.Fill = gradientBrush;
            var installedInfo = CheckInstalled(Constants.ApplicationName);
            if (installedInfo.IsInstalled)
            {
                latestUploadedModel = GetLatestInstalledVersion();
                InstalledLocation = installedInfo.InstalledLocation;
                StartGrid.Visibility = Visibility.Collapsed;
                ButtonGrid.Visibility = Visibility.Collapsed;
                if (latestUploadedModel != null && latestUploadedModel?.Version != null && new Version(latestUploadedModel?.Version).CompareTo(new Version(installedInfo.Version)) > 0)
                {
                    UpdateBtnBorder.Visibility = Visibility.Visible;
                    VersionInfoTextBlock.Text = $"Installed Version  :  {installedInfo.Version}";
                    UpdateVersionInfoTextBlock.Text = $"Latest Version  :  {latestUploadedModel.Version}";
                    UpdateVersionInfoTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    VersionInfoTextBlock.Text = $"Installed Version  :  {installedInfo.Version}";
                }
                alreadyInstalled.Visibility = Visibility.Visible;
               
            }
            MainGrid.DataContext = this;
        }

        private SocialDominatorModel? GetLatestInstalledVersion()
        {
            try
            {
                return helper.GetListNodes<Dictionary<string, SocialDominatorModel>>(Constants.GetDownloadNode(ConfigMode.Release)).LastOrDefault().Value as SocialDominatorModel;
            }
            catch { return new SocialDominatorModel(); }
        }

        private bool isCurrentUser = true;
        public static ReleaseUI GetInstance => instance ?? (instance = new ReleaseUI());
        private int NxtButtonCount = 1;
        private int BackButtonCount = 1;
        public event PropertyChangedEventHandler PropertyChanged;
        private static string UninstallString = string.Empty;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _defaultPath = Constants.GetDefaultIntallationPath;
        public string DefaultPath
        {
            get => _defaultPath;
            set
            {
                _defaultPath = value;
                OnPropertyChanged(nameof(DefaultPath));
            }
        }
        private double _step1Opacity = 1.0;
        public double Step1Opacity
        {
            get { return _step1Opacity; }
            set
            {
                if (_step1Opacity != value)
                {
                    _step1Opacity = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _step2Opacity = 0.5;

        public double Step2Opacity
        {
            get { return _step2Opacity; }
            set
            {
                if (_step2Opacity != value)
                {
                    _step2Opacity = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _step3Opacity = 0.5;

        public double Step3Opacity
        {
            get { return _step3Opacity; }
            set
            {
                if (_step3Opacity != value)
                {

                    _step3Opacity = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _step4Opacity = 0.5;

        public double Step4Opacity
        {
            get { return _step4Opacity; }
            set
            {
                if (_step4Opacity != value)
                {
                    _step4Opacity = value;
                    OnPropertyChanged();
                }
            }
        }

        private int step1Width = 40;

        public int Step1Width
        {
            get { return step1Width; }
            set { step1Width = value; OnPropertyChanged(); }
        }
        private int step2Width = 20;

        public int Step2Width
        {
            get { return step2Width; }
            set { step2Width = value; OnPropertyChanged(); }
        }

        private int step3Width = 20;

        public int Step3Width
        {
            get { return step3Width; }
            set { step3Width = value; OnPropertyChanged(); }
        }

        private int step4Width = 20;

        public int Step4Width
        {
            get { return step4Width; }
            set { step4Width = value; OnPropertyChanged(); }
        }
        Color color1 = (Color)ColorConverter.ConvertFromString("#B0CB0E");
        Color color2 = (Color)ColorConverter.ConvertFromString("#38BBC8");

        public LinearGradientBrush gradientBrush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 0)
        };

        private double _cancelBtnOpacity = 1.0;

        public double CancelBtnOpacity
        {
            get { return _cancelBtnOpacity; }
            set
            {
                if (_cancelBtnOpacity != value)
                {
                    _cancelBtnOpacity = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _backBtnOpacity = 1.0;

        public double BackBtnOpacity
        {
            get { return _backBtnOpacity; }
            set
            {
                if (_backBtnOpacity != value)
                {
                    _backBtnOpacity = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _nextBtnOpacity = 1.0;

        public double NextBtnOpacity
        {
            get { return _nextBtnOpacity; }
            set
            {
                if (_nextBtnOpacity != value)
                {
                    _nextBtnOpacity = value;
                    OnPropertyChanged();
                }
            }
        }
        private static string InstalledLocation = string.Empty;
        public InstalledInfo CheckInstalled(string findByName)
        {
            #region Commented_NotRequiredForBraveExeInstallation
            string[] info = new string[3];
            var installedInfo = new InstalledInfo();
            try
            {
                var registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                //64 bits computer
                RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
                RegistryKey key = key64.OpenSubKey(registryKey);
                if (key != null)
                {
                    foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                    {
                        if (subkey.GetValue("DisplayName") is string displayName && displayName.Equals(findByName))
                        {
                            installedInfo.DisplayName = displayName;

                            installedInfo.InstalledLocation = subkey.GetValue("InstallLocation").ToString();
                            installedInfo.UninstallString = subkey.GetValue("UninstallString").ToString();

                            installedInfo.Version = GetProductVersion(subkey.GetValue("DisplayVersion").ToString());
                            installedInfo.IsInstalled = true;
                            break;
                        }
                    }
                    key.Close();
                }
                if (!installedInfo.IsInstalled)
                {
                    registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                    key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                    key = key64.OpenSubKey(registryKey);

                    if (key != null)
                    {
                        foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                        {
                            if (subkey.GetValue("DisplayName") is string displayName && displayName.Equals(findByName))
                            {
                                installedInfo.DisplayName = displayName;

                                installedInfo.InstalledLocation = subkey.GetValue("InstallLocation").ToString();
                                installedInfo.UninstallString = subkey.GetValue("UninstallString").ToString();

                                installedInfo.Version = GetProductVersion(subkey.GetValue("DisplayVersion").ToString());
                                installedInfo.IsInstalled = true;
                                break;
                            }
                        }
                        key.Close();
                    }

                }

            }
            catch (Exception)
            {

            }
            return installedInfo;
            #endregion
        }
        private async void click_NextBtn(object sender, RoutedEventArgs e)
        {
            if (NxtButtonCount == 1)
            {
                StartGrid.Visibility = Visibility.Collapsed;
                DownloadOptionGrid.Visibility = Visibility.Visible;
                BackBtnBorder.Visibility = Visibility.Visible;
                Step1Width = 20;
                Step1Opacity = 0.5;
                Step2Width = 40;
                Step2Opacity = 1;
                NxtButtonCount++;
                BackButtonCount = 1;
            }
            else if (NxtButtonCount == 2)
            {
                if (string.IsNullOrEmpty(DefaultPath) && !BrowseFolder())
                    return;
                var path = $"{DefaultPath}\\{Constants.ApplicationName}";
                var Created = DirectoryUtility.CreateDirectory(path, true);
                if (!Created)
                {
                    txt_message.Visibility = Visibility.Visible;
                    txt_message.Text = "Access Is Denied At Selected Location,Please Select Another Location";
                    return;
                }
                DefaultPath = path;
                txt_message.Visibility = Visibility.Collapsed;
                DownloadOptionGrid.Visibility = Visibility.Collapsed;
                ConfirmationGrid.Visibility = Visibility.Visible;
                Step2Width = 20;
                Step2Opacity = 0.5;
                Step3Width = 40;
                Step3Opacity = 1;
                BackButtonCount = 2;
                NxtButtonCount++;
            }
            else if (NxtButtonCount == 3)
            {
                ConfirmationGrid.Visibility = Visibility.Collapsed;
                Step3Width = 20;
                Step3Opacity = 0.5;
                Step4Width = 40;
                Step4Opacity = 1;
                CancelBtnOpacity = 1;
                CancelBtn.IsEnabled = true;
                BackBtnOpacity = 0.5;
                BackBtn.IsEnabled = false;
                NextBtnOpacity = 0.5;
                btn_Next.IsEnabled = false;
                InstallingGrid.Visibility = Visibility.Visible;
                if (!Constants.IsCloudBucket)
                {
                    installingProgressStyle.Visibility = Visibility.Visible;
                    InstallingStatusTextBlock.Text = $"Downloading {Constants.ApplicationName}....";
                    if (await helper.DownloadSetupAsync(Constants.GetDownloadNode(ConfigMode.Release), DefaultPath, installingProgressStyle, InstallingStatusTextBlock))
                    {
                        await Task.Delay(1000);
                        InstallingStatusTextBlock.Text = $"Installing {Constants.ApplicationName}....";
                        await InstallMSI();
                        InstallingGrid.Visibility = Visibility.Collapsed;
                        InstallationCompleteGrid.Visibility = Visibility.Visible;
                        CancelBtnBorder.Visibility = Visibility.Collapsed;
                        BackBtnBorder.Visibility = Visibility.Collapsed;
                        NextBtnBorder.Visibility = Visibility.Collapsed;
                        CloseBtnBorder.Visibility = Visibility.Visible;
                        buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                }
                else
                {
                    InstallSocinator();
                }
                NxtButtonCount++;
            }
        }

        private async Task InstallMSI()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    isCurrentUser = (bool)JustMeBtn.IsChecked;
                });
                await CreateRegistry();
                await CreateShortCut();
                #region Code To Install By MSI.
                //string msiPath = $"{DefaultPath}\\{Constants.ApplicationName}New.msi";
                //var path = string.Empty;
                //if (!DefaultPath.Contains(Constants.GetDefaultIntallationPath))
                //    path = $" TARGETDIR={DefaultPath}";
                //Process process = new Process();
                //process.StartInfo.FileName = "msiexec.exe";
                //process.StartInfo.Arguments = $"/i \"{msiPath}\" /quiet{path}";
                //process.StartInfo.UseShellExecute = false;
                //process.StartInfo.RedirectStandardOutput = true;
                //process.StartInfo.RedirectStandardError = true;
                //process.StartInfo.CreateNoWindow = true;

                //process.Start();
                //while (!process.HasExited && installingProgressStyle.Value < installingProgressStyle.Maximum)
                //{
                //    await Task.Delay(150);
                //    installingProgressStyle.Value += 1;
                //}
                //// Capture the output and errors if any
                //string output = process.StandardOutput.ReadToEnd();
                //string errors = process.StandardError.ReadToEnd();
                //process.WaitForExit();

                //while (installingProgressStyle.Value < installingProgressStyle.Maximum)
                //{
                //    await Task.Delay(150);
                //    installingProgressStyle.Value += 1;
                //}
                //await Task.Delay(500);
                //if (!string.IsNullOrEmpty(errors))
                //{

                //}
                #endregion
            }
            catch { }
        }

        private async Task CreateShortCut()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var iconPath = Path.Combine(DefaultPath, Constants.IconFileName);
                        var targetPath = Path.Combine(DefaultPath,$"{Constants.ApplicationName}.exe");
                        var desktopPath = Environment.GetFolderPath(!isCurrentUser ? Environment.SpecialFolder.CommonDesktopDirectory : Environment.SpecialFolder.Desktop);
                        var startmenu = Environment.GetFolderPath(!isCurrentUser ? Environment.SpecialFolder.CommonStartMenu : Environment.SpecialFolder.StartMenu);
                        SaveShortCut(desktopPath, targetPath, iconPath);
                        SaveShortCut(startmenu, targetPath, iconPath);
                    }
                    catch(Exception e)
                    {
                    
                    }
                });
            }
            catch { }
        }

        private void SaveShortCut(string shortCutPath, string exePath, string iconPath)
        {
            var shortcutName = Path.Combine(shortCutPath, $"{Constants.ApplicationName}.lnk");
            try
            {
                //var wshShell = new WshShell();
                //var shortcut = (IWshShortcut)wshShell.CreateShortcut(Path.Combine(shortCutPath, $"{Constants.ApplicationName}.lnk"));
                //shortcut.TargetPath = exePath;
                //shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
                //shortcut.Description = Constants.ShortCutDescription;
                //shortcut.IconLocation = iconPath;
                //shortcut.WindowStyle = 1;
                //shortcut.Save();

                using var shortcut1 = new WindowsShortcut
                {
                    Path = exePath,
                    Description = Constants.ShortCutDescription,
                    IconLocation = iconPath,
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                };
                shortcut1.Save(shortcutName);
            }
            catch { }
        }

        private async Task CreateRegistry()
        {
            await Task.Run(() =>
            {
                //var dest = $"{Constants.InstallerFolder}\\{Constants.AssemblyName}.exe";
                //FileUtilities.CopyFiles(Constants.GetInstallerExe, dest);
                var dest = Constants.GetInstallerExe;
                using (RegistryKey parent = (!isCurrentUser ? Registry.LocalMachine : Registry.CurrentUser).OpenSubKey(
                             @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
                {
                    try
                    {
                        RegistryKey key = null;

                        try
                        {
                            string guidText = Guid.NewGuid().ToString("B");
                            key = parent.OpenSubKey($"{Constants.ApplicationName}", true) ??
                                  parent.CreateSubKey($"{Constants.ApplicationName}");
                            Assembly asm = GetType().Assembly;
                            Version v = asm.GetName().Version;
                            var appName = $"{DefaultPath}\\{Constants.ApplicationName}.exe";
                            string exe = "\"" + appName.Replace("/", "\\\\") + "\"";
                            var versionInfo = FileVersionInfo.GetVersionInfo(appName);
                            var folderSizeInBytes = FileUtilities.GetDirectorySize($"{DefaultPath}");
                            var productVersion = GetProductVersion(versionInfo.ProductVersion.ToString());
                            key.SetValue("DisplayName", Constants.ApplicationName);
                            key.SetValue("version", productVersion);
                            key.SetValue("Publisher", "Globussoft");
                            key.SetValue("EstimatedSize", (int)(folderSizeInBytes/1024), RegistryValueKind.DWord);
                            key.SetValue("DisplayIcon", exe);
                            key.SetValue("DisplayVersion", productVersion);
                            key.SetValue("Contact", "https://socinator.com/contact-us/");
                            key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                            key.SetValue("InstallLocation", $"{DefaultPath}");
                            key.SetValue("UninstallString", dest);
                        }
                        catch (Exception e)
                        {

                        }
                        finally
                        {
                            if (key != null)
                            {
                                key.Close();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });
        }

        private string? GetProductVersion(string version)
        {
            try
            {
                var ver = version?.Split('+');
                return ver?.FirstOrDefault();
            }
            catch { return version; }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BackButtonCount == 2)
            {
                ConfirmationGrid.Visibility = Visibility.Collapsed;
                DownloadOptionGrid.Visibility = Visibility.Visible;
                Step3Width = 20;
                Step3Opacity = 0.5;
                Step2Width = 40;
                Step2Opacity = 1;
                NxtButtonCount = 2;
                BackButtonCount--;
            }
            else if (BackButtonCount == 1)
            {
                DownloadOptionGrid.Visibility = Visibility.Collapsed;
                StartGrid.Visibility = Visibility.Visible;
                BackBtnBorder.Visibility = Visibility.Collapsed;
                Step1Width = 40;
                Step1Opacity = 1;
                Step2Width = 20;
                Step2Opacity = 0.5;
                NxtButtonCount = 1;
                BackButtonCount = 0;
            }
        }
        string filename = "";
        private void InstallSocinator()
        {
            Task.Factory.StartNew(async () =>
            {
                this.Dispatcher.Invoke(() => filename = $"{DefaultPath}\\{Constants.ApplicationName}.exe");
                try
                {
                    await Task.Delay(2000);
                    FileUtilities.CreateFile(filename);
                    WebClient wc = new WebClient();
                    wc.DownloadFileAsync(Constants.uri, filename);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                }
                catch (Exception)
                {
                    txt_message.Text = "Failed to Download/Install.";
                }
            });
        }
        private async void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            await App.Current.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    installingProgressStyle.Value = e.ProgressPercentage;
                    if (installingProgressStyle.Value == installingProgressStyle.Maximum)
                    {

                    }
                }
                catch (Exception)
                {
                    installingProgressStyle.Value = 0;
                }
            });
        }
        private async void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            await App.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (e.Error == null)
                {
                    await Task.Delay(1000);
                    InstallingGrid.Visibility = Visibility.Collapsed;
                    InstallationCompleteGrid.Visibility = Visibility.Visible;
                    CancelBtnBorder.Visibility = Visibility.Collapsed;
                    BackBtnBorder.Visibility = Visibility.Collapsed;
                    NextBtnBorder.Visibility = Visibility.Collapsed;
                    CloseBtnBorder.Visibility = Visibility.Visible;
                    buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                }
            });
        }

        private async void click_CloseBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)LaunchCheckBox.IsChecked)
                {
                    await Task.Run(() => Process.Start(Path.Combine(DefaultPath,$"{Constants.ApplicationName}.exe")));
                }
            }
            catch { }
            Application.Current.Shutdown();
        }
        private async void click_Update(object sender, RoutedEventArgs e)
        {
            bool isOpen = await CheckByProcess();
            var IsOkClicked = !isOpen || DialogUtility.ShowMessageBoxModel(msg: Constants.ConfirmationMessageForClosing,UI:RootBorder);
            if (IsOkClicked)
            {
                AlreadyInstalledTextBlock.Text = $"Updating {Constants.ApplicationName}....";
                VersionInfoTextBlock.Visibility = Visibility.Collapsed;
                UninstallationStatusTextblock.Text = $"Downloading {Constants.ApplicationName}....";
                UninstallingProgressStackPanel.Visibility = Visibility.Visible;
                btn_Uninstall.IsEnabled = false;
                btn_Update.IsEnabled = false;
                await UnInstallByRegistry();
                await Task.Delay(1000);
                if (!Constants.IsCloudBucket)
                {
                    DefaultPath = InstalledLocation;
                    var Created = DirectoryUtility.CreateDirectory(DefaultPath, true);
                    if (await helper.DownloadSetupAsync(Constants.GetDownloadNode(ConfigMode.Release), DefaultPath, UninstallingProgressStyle, UninstallationStatusTextblock))
                    {
                        await Task.Delay(1000);
                        UninstallationStatusTextblock.Text = $"Updating {Constants.ApplicationName}....";
                        await InstallMSI();
                    }
                }
                else
                {
                    InstallSocinator();
                }
                InstallationCompleteTextBlock.Text = "Updation Completed";
                InstallationCompleteStatusTextBlock.Text = "Socinator has been successfully updated";
                LaunchCheckBox.Visibility = Visibility.Collapsed;
                alreadyInstalled.Visibility = Visibility.Collapsed;
                InstallationCompleteGrid.Visibility = Visibility.Visible;
                ButtonGrid.Visibility = Visibility.Visible;
                SliderStackPanel.Visibility = Visibility.Collapsed;
                buttonStackPanel.Visibility = Visibility.Visible;
                CancelBtnBorder.Visibility = Visibility.Collapsed;
                NextBtnBorder.Visibility = Visibility.Collapsed;
                CloseBtnBorder.Visibility = Visibility.Visible;
                buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
        private async void click_Uninstall(object sender, RoutedEventArgs e)
        {
            bool isOpen = await CheckByProcess();
            var IsOkClicked = !isOpen || DialogUtility.ShowMessageBoxModel(msg: Constants.ConfirmationMessageForClosing,UI:RootBorder);
            if (IsOkClicked)
            {
                AlreadyInstalledTextBlock.Text = $"Uninstalling {Constants.ApplicationName}....";
                UpdateVersionInfoTextBlock.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(UninstallString) && UninstallString.Contains("MsiExec"))
                {
                    await ExecuteUninstallString();
                    UninstallingProgressStackPanel.Visibility = Visibility.Visible;
                    btn_Uninstall.IsEnabled = false;
                    while (UninstallingProgressStyle.Value < UninstallingProgressStyle.Maximum)
                    {
                        await Task.Delay(80);
                        UninstallingProgressStyle.Value += 1;
                    }
                    await Task.Delay(500);
                }
                else
                {
                    btn_Uninstall.IsEnabled = false;
                    await UnInstallByRegistry();
                    UninstallingProgressStackPanel.Visibility = Visibility.Visible;
                    while (UninstallingProgressStyle.Value < UninstallingProgressStyle.Maximum)
                    {
                        await Task.Delay(80);
                        UninstallingProgressStyle.Value += 1;
                    }
                    await Task.Delay(500);
                }
                alreadyInstalled.Visibility = Visibility.Collapsed;
                InstallationCompleteTextBlock.Text = "Uninstallation Completed";
                InstallationCompleteStatusTextBlock.Text = "Socinator has been successfully uninstalled";
                LaunchCheckBox.Visibility = Visibility.Collapsed;
                InstallationCompleteGrid.Visibility = Visibility.Visible;
                ButtonGrid.Visibility = Visibility.Visible;
                SliderStackPanel.Visibility = Visibility.Collapsed;
                buttonStackPanel.Visibility = Visibility.Visible;
                CancelBtnBorder.Visibility = Visibility.Collapsed;
                NextBtnBorder.Visibility = Visibility.Collapsed;
                CloseBtnBorder.Visibility = Visibility.Visible;
                buttonStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private async Task UnInstallByRegistry()
        {
            try
            {
                #region Remove Registry.
                string InstallerRegLoc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
                try
                {
                    RegistryKey homeKey = (Registry.CurrentUser).OpenSubKey(InstallerRegLoc, true);
                    RegistryKey appSubKey = homeKey.OpenSubKey(Constants.ApplicationName);
                    if (appSubKey != null)
                        homeKey.DeleteSubKey(Constants.ApplicationName);
                }
                catch { }
                try
                {
                    RegistryKey homeKeyforAllUser = (Registry.LocalMachine).OpenSubKey(InstallerRegLoc, true);
                    RegistryKey appSubKeyforAllUser = homeKeyforAllUser.OpenSubKey(Constants.ApplicationName);
                    if (appSubKeyforAllUser != null)
                        homeKeyforAllUser.DeleteSubKey(Constants.ApplicationName);
                }
                catch { }
                #endregion

                #region Remove ShortCut
                var appName = Constants.ApplicationName;
                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                    string startmenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                    //string taskbar = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar";
                    var dlink = Path.Combine(desktopPath, $"{appName}.lnk");
                    var slink = Path.Combine(startmenu, $"{appName}.lnk");
                    //string tlink = System.IO.Path.Combine(taskbar, $"{appName}.lnk");
                    FileUtilities.DeleteFile(dlink);
                    FileUtilities.DeleteFile(slink);
                    //FileUtilities.DeleteFile(tlink);
                }
                catch { }
                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string startmenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                    //string taskbar = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar";
                    var dlink = Path.Combine(desktopPath, $"{appName}.lnk");
                    var slink = Path.Combine(startmenu, $"{appName}.lnk");
                    //string tlink = System.IO.Path.Combine(taskbar, $"{appName}.lnk");
                    FileUtilities.DeleteFile(dlink);
                    FileUtilities.DeleteFile(slink);
                    //FileUtilities.DeleteFile(tlink);
                }
                catch { }
                #endregion

                #region Remove Directory
                try
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            if (Directory.Exists(InstalledLocation))
                            {
                                DeleteFilesAndDirectory(InstalledLocation);
                            }
                            else
                            {
                                //Delete Files For for all users.
                                var allUsers = await GetAllSystemUsers();
                                foreach (var user in allUsers)
                                {
                                    var unInstallString = "";
                                    if (Directory.Exists(unInstallString))
                                        DeleteFilesAndDirectory(unInstallString);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    });
                }
                catch { }
                finally
                {
                    FileUtilities.DeleteFile($"{Constants.InstallerFolder}\\{Constants.AssemblyName}.exe");
                    DirectoryUtility.DeleteDirectory(InstalledLocation);
                }
                #endregion
            }
            catch { }
        }

        private void DeleteFilesAndDirectory(string uninstallString)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(uninstallString);
                var files = dirInfo.GetFiles();
                foreach (FileInfo file in files)
                    FileUtilities.DeleteFile(file.FullName);
                var dirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                    DirectoryUtility.DeleteDirectory(dir.FullName);
            }
            catch { }
        }

        private async Task<List<string>> GetAllSystemUsers()
        {
            var allUsers = new List<string>();
            try
            {
                using (var context = new PrincipalContext(ContextType.Machine))
                {
                    // Define the user principal object
                    using (var userPrincipal = new UserPrincipal(context))
                    {
                        // Define a PrincipalSearcher to search for users
                        using (var searcher = new PrincipalSearcher(userPrincipal))
                        {
                            // Execute the search and display each found user
                            foreach (var result in searcher.FindAll())
                            {
                                // Cast the result to UserPrincipal to access user details
                                var user = result as UserPrincipal;
                                allUsers.Add(user.SamAccountName);
                            }
                        }
                    }
                }
            }
            catch { }
            return allUsers;
        }

        private async Task<bool> CheckByProcess()
        {
            try
            {
                return Process.GetProcessesByName(Constants.ApplicationName).Any(x => x.ProcessName == Constants.ApplicationName) || Process.GetProcesses().Any(x => x.ProcessName == Constants.ApplicationName);   
            }
            catch
            { return false; }
        }
        private async Task ExecuteUninstallString()
        {
            try
            {
                if (!UninstallString.Contains("/quiet") && !UninstallString.Contains("/silent"))
                {
                    UninstallString += " /quiet /norestart"; // Adjust flags as needed
                }
                UninstallString = UninstallString.Replace("/I", "/X");
                // Add silent/unattended flags to the uninstall string if needed
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {UninstallString}",
                    CreateNoWindow = true, // Do not create a window
                    UseShellExecute = false, // Do not use the OS shell to start the process
                    WindowStyle = ProcessWindowStyle.Hidden, // Ensure no window is shown
                    RedirectStandardOutput = true, // Redirect output to avoid showing anything
                    RedirectStandardError = true // Redirect errors to avoid showing anything
                };

                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit(); // Wait for the process to complete

                    // Optionally capture and log output/error for debugging
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                }

            }
            catch (Exception)
            {

            }
        }
        private void click_Cancel(object sender, RoutedEventArgs e)
        {
            DialogUtility.ShowMessageBoxModel(msg: "Are you sure you want to cancel the installation ?",UI:RootBorder);
        }

        private void click_CancelBtn(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void DiskCostBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var RequiredDiskSpaceSize = 200;
                ReleaseUserControl.Opacity = 0.5;
                DiskSelectionDialog dialog = new DiskSelectionDialog(RequiredDiskSpaceSize);
                dialog.Closing += (s, e) =>
                {
                    ReleaseUserControl.Opacity = 1;
                };
                dialog.Owner = MainWindow.GetInstance;
                dialog.ShowDialog();
                dialog.Activate();
            }
            catch (Exception ex) { }
        }
        public bool BrowseFolder()
        {
            try
            {
                var browser = new WPFFolderBrowser.WPFFolderBrowserDialog()
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    ShowHiddenItems = true,
                    Title = $"Select Folder To Install {Constants.ApplicationName}",
                };
                var result = browser.ShowDialog(Application.Current.MainWindow);
                if(result != null && result == true)
                    DefaultPath = browser.FileName;
                //System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                //dialog.ShowNewFolderButton = true;
                //dialog.Description = "Choose Installation Path";
                //dialog.RootFolder = Environment.SpecialFolder.Desktop;
                //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    DefaultPath = dialog.SelectedPath;
                //}
            }
            catch { }
            return !string.IsNullOrEmpty(DefaultPath);
        }
        private void Browsebtn_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder();
        }
    }
}
