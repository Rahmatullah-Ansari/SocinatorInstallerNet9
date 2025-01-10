using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using SocinatorInstaller.Models;
using System.IO;
using System.Reflection;

namespace SocinatorInstaller.Utilities
{
    public class FileUtilities
    {
        public static bool CreateFile(string FilePath)
        {
            try
            {
                if (!File.Exists(FilePath))
                    File.Create(FilePath).Close();
                return true;
            }
            catch { return false; }
        }
        public static string GetTempInstallationFile()
        {
            DirectoryUtility.CreateDirectory(Constants.InstallerFolder);
            return $"{Constants.InstallerFolder}\\{Constants.ApplicationName}.zip";
        }
        public static string GetCurrentDirectory => Directory.GetCurrentDirectory();
        public static FireBaseAuthCred GetAuthCred()
        {
            try
            {
                return JsonConvert.DeserializeObject<FireBaseAuthCred>(ReadResource($"SocinatorInstaller.FireBaseConfigGLB.json"));
            }
            catch { return new FireBaseAuthCred(); }
        }
        public static string ReadResource(string ResourceName)
        {
            var json = string.Empty;
            Stream stream = null;
            try
            {
                using (stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
            }
            catch { }
            finally
            {
                stream?.Close();
                stream?.Dispose();
            }
            return json;
        }
        public static IFirebaseConfig GetFirebaseConfig()
        {
            var config = GetAuthCred();
            return new FirebaseConfig
            {
                AuthSecret = config.AuthSecret,
                BasePath = config.BasePath
            };
        }
        public static string ReadServiceAccountConfig()
        {
            var json = string.Empty;
            try
            {
                json = ReadResource($"SocinatorInstaller.ServiceAccountGLB.json");
            }
            catch { }
            return json;
        }
        public static string GetServiceAccountFileName => $"{GetCurrentDirectory}/ServiceAccountGLB.json";
        public static void SaveConfig(ProductInfo productInfo)
        {
            try
            {
                File.WriteAllText($"{GetCurrentDirectory}/{productInfo.ProductConfig}.txt", JsonConvert.SerializeObject(productInfo));
            }
            catch { }
        }
        public static ProductInfo GetConfig(string Config)
        {
            try
            {
                return JsonConvert.DeserializeObject<ProductInfo>(File.ReadAllText($"{GetCurrentDirectory}/{Config}.txt"));
            }
            catch { return new ProductInfo(); }
        }
        public static string GetDownloadPath(string Config, string Version)
        {
            var downloadPath = Path.Combine(GetCurrentDirectory, Constants.ApplicationName);
            DirectoryUtility.CreateDirectory(downloadPath);
            downloadPath = Path.Combine(downloadPath, Config);
            DirectoryUtility.CreateDirectory(downloadPath);
            downloadPath = Path.Combine(downloadPath, $"{Constants.ApplicationName}_{Version.Replace(".", "_")}");
            DirectoryUtility.CreateDirectory(downloadPath);
            return downloadPath;
        }
        public static bool DeleteFile(string FilePath)
        {
            try
            {
                File.Delete(FilePath);
                return true;
            }
            catch { return false; }
        }
        public  static long GetDirectorySize(string folderPath)
        {
            long size = 0;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

                // Add file sizes
                foreach (FileInfo file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    size += file.Length;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
            }
            catch (Exception ex)
            {
            }
            return size;
        }
        public static bool CopyFiles(string source, string dest, bool Overwrite = true)
        {
            try
            {
                File.Copy(source, dest, Overwrite);
                return true;
            }
            catch { return false; }
        }

        public static void CopyInstaller(string dest)
        {
            try
            {
                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SocinatorInstaller.UnInstaller.exe"))
                {
                    if (resourceStream != null)
                    {
                        using (var fileStream = new FileStream(dest, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
