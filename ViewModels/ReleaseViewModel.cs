namespace SocinatorInstaller.ViewModels
{
    public class ReleaseViewModel
    {
        private static ReleaseViewModel instance;
        public static ReleaseViewModel GetInstance => instance ?? (instance = new ReleaseViewModel());
    }
}
