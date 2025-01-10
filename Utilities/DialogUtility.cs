using SocinatorInstaller.Views;
using System.Windows;
using System.Windows.Media;

namespace SocinatorInstaller.Utilities
{
    public class DialogUtility
    {
        public static bool ShowMessageBoxModel(bool isyes = false, string msg = "", bool isAsync = false,UIElement UI=null)
        {
            var IsOkClicked = false;
            CustomMessageBox msgBox = new CustomMessageBox(isyes, msg);
            RectangleGeometry rect = new RectangleGeometry();
            rect.Rect = new Rect(0, 50, 300, 108);
            rect.RadiusX = 10;
            rect.RadiusY = 10;
            Window window = new Window
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                BorderThickness = new Thickness(0),
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                Height = 160,
                Width = 300,
                Background = Brushes.Transparent,
                Clip = rect
            };
            window.Owner = MainWindow.GetInstance;
            msgBox.MyWindow = window;
            window.Content = msgBox;
            if (UI != null)
                UI.Opacity = Constants.UIOpacityDisable;
            window.Closing += (s, e) =>
            {
                IsOkClicked = msgBox.IsOk;
                if (UI != null)
                    UI.Opacity = Constants.UIOpacityEnable;
            };
            window.ShowDialog();
            window.Activate();
            return IsOkClicked;
        }
    }
}
