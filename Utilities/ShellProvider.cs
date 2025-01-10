using System.Runtime.InteropServices;

namespace SocinatorInstaller.Utilities
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    public interface IShellLink
    {
        void SetPath([In, MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void SetArguments([In, MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void SetWorkingDirectory([In, MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void SetIconLocation([In, MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    public class ShellLink { }
}
