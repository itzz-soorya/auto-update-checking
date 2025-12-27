# Auto Update Demo

A simple WPF application demonstrating automatic updates from GitHub releases.

## Features

- ✅ Modern WPF UI with clean design
- ✅ Automatic update checking on startup
- ✅ Manual update checking via button
- ✅ Downloads and installs updates from GitHub releases
- ✅ Version comparison and notification

## Setup Instructions

### 1. Configure Your GitHub Repository

Open `MainWindow.xaml.cs` and update line 17 with your GitHub details:

```csharp
_autoUpdater = new AutoUpdater("YOUR_GITHUB_USERNAME", "YOUR_REPO_NAME");
```

Replace:
- `YOUR_GITHUB_USERNAME` with your GitHub username
- `YOUR_REPO_NAME` with your repository name

### 2. Build the Application

```bash
dotnet build -c Release
```

The executable will be in: `bin\Release\net8.0-windows\AutoUpdateDemo.exe`

### 3. Create a Release on GitHub

1. Go to your GitHub repository
2. Click on "Releases" → "Create a new release"
3. Create a tag (e.g., `v1.0.0`)
4. Upload your built `.exe` file as an asset
5. Publish the release

### 4. Test the Auto-Update

1. Run the application (v1.0.0)
2. Modify some code (e.g., change the welcome text)
3. Update version in `AutoUpdateDemo.csproj` to `1.0.1`:
   ```xml
   <Version>1.0.1</Version>
   ```
4. Rebuild the application
5. Create a new GitHub release (v1.0.1) and upload the new `.exe`
6. Run the old version (v1.0.0)
7. The app will detect the new version and prompt you to update!

## How It Works

1. **On Startup**: The app checks GitHub releases API for the latest version
2. **Version Comparison**: Compares current version with latest release
3. **Notification**: If update available, shows a dialog
4. **Download**: Downloads the new installer from GitHub release assets
5. **Install**: Launches the new installer and closes the current app

## Project Structure

```
AutoUpdateDemo/
├── AutoUpdateDemo.csproj    # Project configuration
├── App.xaml                  # Application resources & styles
├── App.xaml.cs              # Application entry point
├── MainWindow.xaml          # Main UI design
├── MainWindow.xaml.cs       # Main window logic
└── AutoUpdater.cs           # Auto-update functionality
```

## Version Format

Use semantic versioning: `Major.Minor.Build` (e.g., 1.0.0, 1.0.1, 1.1.0)

GitHub release tags should be: `v1.0.0` or `1.0.0`

## Requirements

- .NET 8.0 SDK or later
- Windows OS
- GitHub repository with releases

## Building for Distribution

To create a standalone installer, consider using:
- **Inno Setup** - Free installer creator
- **WiX Toolset** - Create MSI installers
- **ClickOnce** - Built-in .NET deployment

## Notes

- The app looks for `.exe` or `.msi` files in GitHub release assets
- Ensure your GitHub releases are public for the API to access them
- The downloaded installer is saved to the Windows temp folder

## License

MIT License - Feel free to use and modify!
