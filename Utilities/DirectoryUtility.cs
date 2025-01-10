using System.IO;

namespace SocinatorInstaller9.Utilities
{
    public class DirectoryUtility
    {
        public static bool CreateDirectory(string Path, bool Overwrite = false)
        {
            try
            {
                if (Overwrite && Directory.Exists(Path))
                    Directory.Delete(Path, true);
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
                return true;
            }
            catch { return false; }
        }
        public static bool DeleteDirectory(string Path)
        {
            try
            {
                Directory.Delete(Path, true);
                return true;
            }
            catch { return false; }
        }
    }
}
