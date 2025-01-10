using SocinatorInstaller.Utilities;
using System.IO;

namespace SocinatorInstaller.Models
{
    public class ProductInfo
    {
        public string ProductName { get; set; }
        public string ProductVersion { get; set; }
        public string ProductConfig { get; set; }
        public string NodeName { get; set; }
        public string FileName { get; set; }
        public ProductInfo(string FilePath, string Config)
        {
            try
            {
                var fileInfo = new FileInfo(FilePath);
                var splitted = fileInfo.Name.Split('_').ToList();
                var productname = splitted.FirstOrDefault()?.Replace(" ", "");
                ProductName = !string.IsNullOrEmpty(productname) && (productname.Contains("Socinator") || productname.Contains("Social")) ?
                    Constants.ApplicationName : productname;
                ProductVersion = splitted.LastOrDefault()?.Replace(".zip", "");
                ProductConfig = Config;
                FileName = fileInfo.Name.Replace(fileInfo.Extension, "").Replace(".", "_").Replace(" ", "") + fileInfo.Extension;
                NodeName = $"{ProductName}_{ProductConfig}_{ProductVersion.Replace(".", "_")}";
            }
            catch { }
        }
        public ProductInfo() { }
    }
}
