using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace AutoUpdateDemo
{
    public partial class MainWindow : Window
    {
        private readonly AutoUpdater _autoUpdater;

        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize auto updater
            _autoUpdater = new AutoUpdater("itzz-soorya", "auto-update-checking");
            
            // Display current version
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText.Text = $"Version {version?.Major}.{version?.Minor}.{version?.Build}";
            
            // Check for updates on startup
            CheckForUpdatesOnStartup();
        }

        private async void CheckForUpdatesOnStartup()
        {
            try
            {
                var updateAvailable = await _autoUpdater.CheckForUpdatesAsync();
                
                if (updateAvailable)
                {
                    var result = MessageBox.Show(
                        $"A new version {_autoUpdater.LatestVersion} is available!\n\n" +
                        $"Current version: {_autoUpdater.CurrentVersion}\n" +
                        $"Latest version: {_autoUpdater.LatestVersion}\n\n" +
                        "Would you like to download and install the update now?",
                        "Update Available",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        await DownloadAndInstallUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent fail on startup - don't bother user unless they explicitly check
                Console.WriteLine($"Auto-update check failed: {ex.Message}");
            }
        }

        private async void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Checking for updates...";
                CheckUpdateButton.IsEnabled = false;

                var updateAvailable = await _autoUpdater.CheckForUpdatesAsync();

                if (updateAvailable)
                {
                    StatusText.Text = $"New version {_autoUpdater.LatestVersion} available!";
                    
                    var result = MessageBox.Show(
                        $"A new version is available!\n\n" +
                        $"Current version: {_autoUpdater.CurrentVersion}\n" +
                        $"Latest version: {_autoUpdater.LatestVersion}\n\n" +
                        "Would you like to download and install the update now?",
                        "Update Available",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        await DownloadAndInstallUpdate();
                    }
                }
                else
                {
                    StatusText.Text = "You are using the latest version!";
                    MessageBox.Show(
                        "You are already using the latest version of the application.",
                        "No Updates Available",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Failed to check for updates.";
                MessageBox.Show(
                    $"Error checking for updates:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                CheckUpdateButton.IsEnabled = true;
            }
        }

        private async Task DownloadAndInstallUpdate()
        {
            try
            {
                StatusText.Text = "Downloading update...";
                
                var downloadPath = await _autoUpdater.DownloadUpdateAsync();
                
                StatusText.Text = "Installing update...";
                
                MessageBox.Show(
                    $"Update downloaded successfully!\n\n" +
                    $"Location: {downloadPath}\n\n" +
                    "The application will now close. Please run the new installer to complete the update.",
                    "Update Ready",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Launch the installer
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = downloadPath,
                    UseShellExecute = true
                });

                // Close the application
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                StatusText.Text = "Failed to download update.";
                MessageBox.Show(
                    $"Error downloading update:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show(
                $"Auto Update Demo Application\n\n" +
                $"Version: {version?.Major}.{version?.Minor}.{version?.Build}\n" +
                $"Built with: .NET 8.0 & WPF\n\n" +
                "This application demonstrates automatic updates from GitHub releases.",
                "About",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
