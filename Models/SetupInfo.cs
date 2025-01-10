using SocinatorInstaller.Enums;
using SocinatorInstaller.Utilities;


namespace SocinatorInstaller.Models
{
    public class SetupInfo : BindableBase
    {
        private string _setuppath;
        private string _title = "Upload Setup To Bucket";
        private bool _progressEnable = false;
        private string _status;
        private List<ConfigMode> _config = new List<ConfigMode> { ConfigMode.Release, ConfigMode.Debug, ConfigMode.Test, ConfigMode.Custom };
        private ConfigMode _selectedConfig = ConfigMode.Release;
        public string SetupPath
        {
            get => _setuppath;
            set => SetProperty(ref _setuppath, value);
        }
        public List<ConfigMode> Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }
        public ConfigMode SelectedConfig
        {
            get => _selectedConfig;
            set => SetProperty(ref _selectedConfig, value);
        }
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        public bool ProgressEnable
        {
            get => _progressEnable;
            set => SetProperty(ref _progressEnable, value);
        }
    }
}
