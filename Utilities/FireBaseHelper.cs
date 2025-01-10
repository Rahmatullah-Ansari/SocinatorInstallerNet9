using Firebase.Storage;
using FirebaseAdmin;
using FireSharp;
using FireSharp.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SocinatorInstaller.Enums;
using SocinatorInstaller.Models;
using SocinatorInstaller.Responses;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;

namespace SocinatorInstaller.Utilities
{
    public class FireBaseHelper
    {
        private static FireBaseHelper instance;
        private readonly StorageService storageService;
        public static FireBaseHelper GetInstance => instance ?? (instance = new FireBaseHelper());
        public FireBaseHelper()
        {
            try
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson(FileUtilities.ReadServiceAccountConfig())
                });
            }
            catch { }
        }
        public static IFirebaseConfig config { get; private set; } = FileUtilities.GetFirebaseConfig();
        public static FirebaseClient client { get; private set; } = new FirebaseClient(config);
        public async Task<T> GetTaskAsync<T>(string Path)
        {
            var response = await client.GetAsync(Path.Replace(".", "_"));
            return response.ResultAs<T>();
        }
        public async Task<T> SetTaskAsync<T>(string Path, T DataModel)
        {
            var response = await client.SetAsync(Path.Replace(".", "_"), DataModel);
            return response.ResultAs<T>();
        }
        public async Task<T> PushTaskAsync<T>(string Path, T DataModel)
        {
            var response = await client.PushAsync(Path.Replace(".", "_"), DataModel);
            return response.ResultAs<T>();
        }
        public async Task<T> GetList<T>(string Path)
        {
            var response = await client.GetAsync(Path.Replace(".", "_"));
            return response.ResultAs<T>();
        }
        public T GetListNodes<T>(string path)
        {
            var response =  client.Get(path.Replace(".", "_"));
            return response.ResultAs<T>();
        }
        public async Task<bool> DownloadSetupAsync(string DataBasePath, string DownloadPath, ProgressBar progressBar, TextBlock status)
        {
            try
            {
                var Nodes = await GetList<Dictionary<string, SocialDominatorModel>>(DataBasePath);
                if (Nodes != null && Nodes.Count > 0)
                {
                    var model = Nodes.LastOrDefault().Value as SocialDominatorModel;
                    using (var httpClient = new HttpClient())
                    {
                        try
                        {
                            // Send a GET request to the download URL
                            var response = await httpClient.GetAsync(model.StoragePath, HttpCompletionOption.ResponseHeadersRead);

                            // Ensure the request was successful
                            response.EnsureSuccessStatusCode();
                            long? totalBytes = response.Content.Headers.ContentLength;
                            var localFilePath = $"{FileUtilities.GetDownloadPath(model.ConfigMode, model.Version)}\\{model.ProductName}_{model.Version.Replace(".", "_")}.zip";
                            // Read the response content as a stream
                            using (var responseStream = await response.Content.ReadAsStreamAsync())
                            {
                                // Save the content to a file
                                using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                                {
                                    byte[] buffer = new byte[8192];
                                    long totalBytesRead = 0;
                                    int bytesRead;
                                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                    {
                                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                                        totalBytesRead += bytesRead;
                                        if (totalBytes.HasValue)
                                        {
                                            double progress = (double)totalBytesRead / totalBytes.Value * 100;
                                            progressBar.Value = progress;
                                            status.Text = $"Downloading {Constants.ApplicationName}.... {progress:F0} %";
                                        }
                                    }
                                    //await responseStream.CopyToAsync(fileStream);
                                }
                            }
                            FileUtilities.SaveConfig(new ProductInfo(localFilePath, ConfigMode.Release.ToString()));
                            status.Text = $"Extracting files please wait...";
                            await UnzipFile(localFilePath, model, DownloadPath);
                            return true;
                        }
                        catch (HttpRequestException httpEx)
                        {
                            Console.WriteLine($"HTTP request error: {httpEx.Message}");
                        }
                        catch (IOException ioEx)
                        {
                            Console.WriteLine($"IO error: {ioEx.Message}");
                        }
                        return false;
                    }
                }
                return false;
            }
            catch { return false; }
        }

        private async Task UnzipFile(string localFilePath, SocialDominatorModel model, string DownloadPath)
        {
            ZipFile file = null;
            try
            {
                var finalKey = await GetPassword(model.PublicKey.ToString());
                var stream = File.OpenRead(localFilePath);
                file = new ZipFile(stream);
                if (!string.IsNullOrEmpty(finalKey))
                    file.Password = finalKey;
                var zipFile = string.Empty;
                foreach (ZipEntry entry in file)
                {
                    try
                    {
                        await Task.Run(() =>
                        {
                            if (entry.IsFile)
                            {
                                var fileName = entry.Name;
                                byte[] buffer = new byte[4096];
                                var zipStream = file.GetInputStream(entry);
                                var fullPath = Path.Combine(DownloadPath, fileName);
                                if (!string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(zipFile) && fileName.Contains(".zip"))
                                    zipFile = fileName;
                                using (FileStream fileStream = File.Create(fullPath))
                                {
                                    StreamUtils.Copy(zipStream, fileStream, buffer);
                                }
                            }
                        });
                    }
                    catch { }
                }
                var ZipPath = $"{DownloadPath}\\{zipFile}";
                await Task.Run(() => System.IO.Compression.ZipFile.ExtractToDirectory(ZipPath, DownloadPath));
                FileUtilities.DeleteFile(ZipPath);
                await ExtractMSIIFFound(DownloadPath);
            }
            catch { }
            finally
            {
                if (file != null)
                {
                    file.IsStreamOwner = true;
                    file.Close();
                }
                FileUtilities.DeleteFile(localFilePath);
            }
        }

        private async Task ExtractMSIIFFound(string downloadPath)
        {
            try
            {
                var files = Directory.GetFiles(downloadPath).ToList();
                if (files != null && files.Count > 0 && files.Any(x => Path.GetExtension(x) == ".msi"))
                {
                    var targetFile = files.FirstOrDefault(x => Path.GetExtension(x) == ".msi");
                    var targetFileName = Path.GetFileName(targetFile);
                    var destinationFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{targetFileName}";
                    File.Move(targetFile, destinationFile);
                    foreach (var file in files.ToList())
                    {
                        FileUtilities.DeleteFile(file);
                    }
                    try
                    {
                        var processStartInfo = new ProcessStartInfo
                        {
                            FileName = "msiexec.exe",
                            Arguments = $"/a \"{destinationFile}\" /qn TARGETDIR=\"{downloadPath}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        using (var process = Process.Start(processStartInfo))
                        {
                            process.WaitForExit();
                            if (process.ExitCode != 0)
                            {
                                throw new InvalidOperationException($"msiexec failed with exit code {process.ExitCode}");
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        FileUtilities.DeleteFile(destinationFile);
                    }
                }
            }
            catch { }
        }

        public async Task<UploadResponseHandler> UploadFileAsync(string FilePath, string BucketName, string FileName)
        {
            try
            {
                var storage = new FirebaseStorage(BucketName);
                var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                var sref = await storage.Child(FileName).PutAsync(stream);
                var downloadUrl = await storage.Child(FileName?.Trim())
                    .GetDownloadUrlAsync();
                return new UploadResponseHandler { UploadedResult = downloadUrl, Success = true, ErrorMessage = string.Empty };
            }

            catch (Exception)
            {
                return new UploadResponseHandler { UploadedResult = string.Empty, Success = false, ErrorMessage = string.Empty }; ;
            }
        }
        public async Task<UploadResponseHandler> UploadSetupFile(string FilePath, ConfigMode configMode = ConfigMode.Release, bool ProtectFile = true)
        {
            try
            {
                var cred = FileUtilities.GetAuthCred();
                var protectedFile = await GetProtectedFile(FilePath, ProtectFile, configMode);
                var productInfo = new ProductInfo(protectedFile.Item1, configMode.ToString());
                var uploadResponse = await UploadFileAsync(protectedFile.Item1, cred.BucketName, productInfo.FileName);
                if (uploadResponse != null && uploadResponse.Success)
                {
                    var UpdateData = new SocialDominatorModel
                    {
                        ProductName = productInfo.ProductName,
                        Version = productInfo.ProductVersion,
                        ConfigMode = productInfo.ProductConfig.ToString(),
                        PublicKey = protectedFile.Item2,
                        ReleaseDate = DateTime.Now,
                        StoragePath = uploadResponse.UploadedResult
                    };
                    var uploaded = await SetTaskAsync<SocialDominatorModel>($"{productInfo.ProductName}/{productInfo.ProductConfig}/{productInfo.ProductVersion}", UpdateData);
                    FileUtilities.SaveConfig(productInfo);
                    FileUtilities.DeleteFile(protectedFile.Item1);
                }
                return uploadResponse;
            }
            catch (Exception ex) { return new UploadResponseHandler { UploadedResult = string.Empty, Success = false, ErrorMessage = ex.Message }; }
        }

        private async Task<(string, string)> GetProtectedFile(string filePath, bool protectFile, ConfigMode configMode)
        {
            var p = string.Empty;
            try
            {
                var fileInfo = new FileInfo(filePath);
                var productinfo = new ProductInfo(filePath, configMode.ToString());
                var outPutFile = $"{FileUtilities.GetCurrentDirectory}\\{fileInfo.Name}";
                using (FileStream zipFileStream = new FileStream(outPutFile, FileMode.Create))
                using (ZipOutputStream zipOutputStream = new ZipOutputStream(zipFileStream))
                {
                    zipOutputStream.SetLevel(9);
                    if (protectFile)
                    {
                        var keyData = await SetPassword(productinfo.ProductVersion);
                        p = keyData.Item2;
                        zipOutputStream.Password = keyData.Item1;
                    }

                    fileInfo = new FileInfo(filePath);
                    string entryName = fileInfo.Name;
                    ZipEntry entry = new ZipEntry(entryName)
                    {
                        DateTime = fileInfo.LastWriteTime,
                        Size = fileInfo.Length
                    };

                    zipOutputStream.PutNextEntry(entry);

                    using (FileStream fileStream = fileInfo.OpenRead())
                    {
                        await Task.Run(() =>
                        {
                            fileStream.CopyTo(zipOutputStream);
                        });
                    }
                    zipOutputStream.CloseEntry();
                    zipOutputStream.Close();
                    zipOutputStream.Dispose();
                    zipFileStream.Close();
                    zipFileStream.Dispose();
                }
                return (outPutFile, p);
            }
            catch { return (filePath, p); }
        }

        private async Task<(string, string)> SetPassword(string version)
        {
            var data = string.Empty;
            try
            {
                var key = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Constants.ApplicationName}_{version.Replace(".", "_")}"));
                using (SHA256 sHA = SHA256.Create())
                {
                    byte[] bytes = sHA.ComputeHash(Encoding.UTF8.GetBytes(key));
                    StringBuilder builder = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                    return (builder.ToString(), Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(builder.ToString())))));
                }
            }
            catch { return (data, ""); }
        }
        private async Task<string> GetPassword(string publicKey)
        {
            var data = string.Empty;
            try
            {
                var key = Convert.FromBase64String(publicKey);
                return Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(key)));
            }
            catch { return data; }
        }
    }
}
