using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AutoUpdateDemo
{
    public class AutoUpdater
    {
        private readonly string _githubOwner;
        private readonly string _githubRepo;
        private readonly HttpClient _httpClient;

        public string CurrentVersion { get; private set; }
        public string LatestVersion { get; private set; }
        public string DownloadUrl { get; private set; }

        public AutoUpdater(string githubOwner, string githubRepo)
        {
            _githubOwner = githubOwner;
            _githubRepo = githubRepo;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AutoUpdateDemo/1.0");
            
            // Get current version from assembly
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";
        }

        /// <summary>
        /// Checks if a new version is available on GitHub releases
        /// </summary>
        public async Task<bool> CheckForUpdatesAsync()
        {
            try
            {
                var apiUrl = $"https://api.github.com/repos/{_githubOwner}/{_githubRepo}/releases/latest";
                var response = await _httpClient.GetStringAsync(apiUrl);
                var releaseData = JObject.Parse(response);

                // Get latest version from tag name (e.g., "v1.0.1" or "1.0.1")
                var tagName = releaseData["tag_name"]?.ToString();
                if (string.IsNullOrEmpty(tagName))
                    return false;

                // Remove 'v' prefix if present
                LatestVersion = tagName.TrimStart('v');

                // Get download URL for the installer (look for .exe or .msi file)
                var assets = releaseData["assets"] as JArray;
                if (assets != null && assets.Count > 0)
                {
                    // Find the first .exe or .msi file
                    var installer = assets.FirstOrDefault(a => 
                    {
                        var name = a["name"]?.ToString()?.ToLower();
                        return name?.EndsWith(".exe") == true || name?.EndsWith(".msi") == true;
                    });

                    if (installer != null)
                    {
                        DownloadUrl = installer["browser_download_url"]?.ToString();
                    }
                }

                // Compare versions
                return CompareVersions(CurrentVersion, LatestVersion) < 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check for updates: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Downloads the latest update to the temp folder
        /// </summary>
        public async Task<string> DownloadUpdateAsync()
        {
            if (string.IsNullOrEmpty(DownloadUrl))
                throw new Exception("No download URL available. Please check for updates first.");

            try
            {
                // Get the file name from URL
                var fileName = Path.GetFileName(new Uri(DownloadUrl).LocalPath);
                var tempPath = Path.Combine(Path.GetTempPath(), fileName);

                // Download the file
                var fileBytes = await _httpClient.GetByteArrayAsync(DownloadUrl);
                await File.WriteAllBytesAsync(tempPath, fileBytes);

                return tempPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to download update: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Compares two version strings (e.g., "1.0.0" vs "1.0.1")
        /// Returns: -1 if version1 < version2, 0 if equal, 1 if version1 > version2
        /// </summary>
        private int CompareVersions(string version1, string version2)
        {
            try
            {
                var v1Parts = version1.Split('.').Select(int.Parse).ToArray();
                var v2Parts = version2.Split('.').Select(int.Parse).ToArray();

                int maxLength = Math.Max(v1Parts.Length, v2Parts.Length);

                for (int i = 0; i < maxLength; i++)
                {
                    int v1Part = i < v1Parts.Length ? v1Parts[i] : 0;
                    int v2Part = i < v2Parts.Length ? v2Parts[i] : 0;

                    if (v1Part < v2Part) return -1;
                    if (v1Part > v2Part) return 1;
                }

                return 0;
            }
            catch
            {
                // If parsing fails, treat as equal
                return 0;
            }
        }
    }
}
