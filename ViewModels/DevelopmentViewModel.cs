using SocinatorInstaller.Views;
using SocinatorInstaller.Models;
using SocinatorInstaller.Utilities;
using System.IO;
using System.IO.Compression;
using System.Windows.Input;

namespace SocinatorInstaller.ViewModels
{
    public class DevelopmentViewModel : BindableBase
    {
        private static DevelopmentViewModel instance;
        public static FireBaseHelper fireBaseHelper { get; set; }
        public static DevelopmentViewModel GetInstance => instance ?? (instance = new DevelopmentViewModel());
        public DevelopmentViewModel()
        {
            fireBaseHelper = FireBaseHelper.GetInstance;
            UploadSetupCommand = new BaseCommand<object>(CanExecute, UploadSetupExecute);
            BrowseFileCommand = new BaseCommand<object>(sender => true, BrowseSetupFile);
            CancelCommand = new BaseCommand<object>(sender => true, CancelAndCloseExecute);
        }
        private SetupInfo _model = new SetupInfo();
        public SetupInfo Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }
        private string _note = $"Note * Select Project Archive File With Format FileName_version.zip (ex- {Constants.ApplicationName}_x.x.x.x.zip)";
        //private string _note = $"Note * Select Project Build Output Path To Build Setup (ex. ...dominatorhouse-social\\DominatorHouse\\bin\\Release)";
        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }
        public ICommand UploadSetupCommand { get; set; }
        public ICommand BrowseFileCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        private bool CanExecute(object arg)
        {
            return Model != null && !string.IsNullOrEmpty(Model.SetupPath);
        }
        private void CancelAndCloseExecute(object obj)
        {
            try
            {
                DialogUtility.ShowMessageBoxModel(msg: $"Do you want to close {Constants.ApplicationName} installer manager?",UI:DeveloperUI.Instance.RootBorder);
            }
            catch { }
        }
        private async void UploadSetupExecute(object obj)
        {
            try
            {
                if (Model != null && Model.Title == "Finish")
                {
                    MainWindow.GetInstance.CloseWindow();
                    return;
                }
                //if (!await BuildSetupFileFromOutputPath(Model.SetupPath))
                //{
                //    DialogUtility.ShowMessageBoxModel(msg: "Invalid Output Path Is Selected Please Select Correct Path", isyes: true);
                //    return;
                //}
                Model.ProgressEnable = true;
                Model.Title = "Uploading...";
                Model.Status = "Uploading To Bucket Please Wait...";
                var UploadResponse = await fireBaseHelper.UploadSetupFile(Model.SetupPath, Model.SelectedConfig, true);
                if (UploadResponse != null && UploadResponse.Success)
                {
                    Model.Status = "Setup Uploaded Successfully.";
                    Model.ProgressEnable = false;
                    Model.Title = "Finish";
                }
                else
                {
                    Model.Status = "Failed To Upload With Error => " + UploadResponse?.ErrorMessage ?? "Check Internet Connnection And Try Again!";
                    Model.ProgressEnable = false;
                    Model.Title = "Failed Try Again.";
                }
            }
            catch (Exception ex)
            {
                Model.Status = "Failed To Upload With Error => " + ex.Message;
                Model.ProgressEnable = false;
                Model.Title = "Failed Try Again.";
            }
        }

        private async Task<bool> BuildSetupFileFromOutputPath(string setupPath)
        {
            try
            {
                var allFiles = Directory.GetFiles(setupPath);
                if (allFiles.Length > 0)
                {
                    if (!allFiles.Any(x => !string.IsNullOrEmpty(x) && x.Contains($"{Constants.ApplicationName}.exe")))
                        return false;
                    Model.SetupPath = FileUtilities.GetTempInstallationFile();
                    FileUtilities.DeleteFile(Model.SetupPath);
                    Model.ProgressEnable = true;
                    Model.Status = "Building Setup....";
                    await Task.Run(() =>
                    {
                        using (ZipArchive zip = ZipFile.Open(Model.SetupPath, ZipArchiveMode.Create))
                        {
                            foreach (var fPath in allFiles)
                            {
                                var fileName = Path.GetFileName(fPath);
                                zip.CreateEntryFromFile(fPath, fileName);
                            }
                        }
                    });
                    return true;
                }
                return false;
            }
            catch { return false; }
            finally
            {
                Model.ProgressEnable = false;
            }
        }

        private void BrowseSetupFile(object obj)
        {
            try
            {
                //FolderBrowserDialog dialog = new FolderBrowserDialog();
                //dialog.RootFolder = Environment.SpecialFolder.Desktop;
                //dialog.Description = "Select Project Output Bin Configuration Path";
                //if(dialog.ShowDialog() == DialogResult.OK)
                //{
                //    Model.SetupPath = dialog.SelectedPath;
                //}
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.DefaultExt = "zip";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                if (dialog.ShowDialog() == true)
                {
                    Model.SetupPath = dialog.FileName;
                }
            }
            catch { }
        }
    }
}
