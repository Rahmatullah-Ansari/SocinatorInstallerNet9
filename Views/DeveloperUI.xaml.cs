using SocinatorInstaller.ViewModels;
using System.Windows.Controls;

namespace SocinatorInstaller.Views
{
    /// <summary>
    /// Interaction logic for DeveloperUI.xaml
    /// </summary>
    public partial class DeveloperUI : UserControl
    {
        private static DeveloperUI instance;
        public DeveloperUI()
        {
            InitializeComponent();
            MainGrid.DataContext = DevelopmentViewModel.GetInstance;
        }
        public static DeveloperUI Instance => instance ?? (instance = new DeveloperUI());
    }
}
