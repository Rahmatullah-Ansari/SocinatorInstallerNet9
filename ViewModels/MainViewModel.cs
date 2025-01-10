using SocinatorInstaller.Utilities;
using SocinatorInstaller.Views;
using System.Windows.Controls;

namespace SocinatorInstaller.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private static MainViewModel instance;
        private UserControl _usercontrol = new UserControl();
        public UserControl SelectedUserControl
        {
            get => _usercontrol;
            set => SetProperty(ref _usercontrol, value);
        }
        public static MainViewModel GetInstance => instance ?? (instance = new MainViewModel());
        public MainViewModel()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            try
            {
                SelectedUserControl = (Constants.DeveloperMode ? DeveloperUI.Instance: ReleaseUI.GetInstance);
            }
            catch (Exception) { SelectedUserControl = ReleaseUI.GetInstance; }
        }
    }
}
