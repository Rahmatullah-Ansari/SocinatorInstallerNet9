using SocinatorInstaller9.Utilities;
using SocinatorInstaller9.Views;
using System;
using System.Windows.Controls;

namespace SocinatorInstaller9.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private static MainViewModel instance;
        private UserControl _usercontrol = new UserControl();
        private int Mode = 0;
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
                //#if DEBUG
                //                Mode = 1;
                //#endif
                SelectedUserControl = (Mode == 0 ? ReleaseUI.GetInstance : DeveloperUI.Instance);
            }
            catch (Exception) { SelectedUserControl = ReleaseUI.GetInstance; }
        }
    }
}
