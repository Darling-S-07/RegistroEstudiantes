This file is a merged representation of the entire codebase, combined into a single document by Repomix.

# File Summary

## Purpose
This file contains a packed representation of the entire repository's contents.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Repository files (if enabled)
5. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
```
Data/
  SQLiteDatabase.cs
Models/
  Estudiante.cs
Platforms/
  Android/
    Resources/
      values/
        colors.xml
    AndroidManifest.xml
    MainActivity.cs
    MainApplication.cs
  iOS/
    Resources/
      PrivacyInfo.xcprivacy
    AppDelegate.cs
    Info.plist
    Program.cs
  MacCatalyst/
    AppDelegate.cs
    Entitlements.plist
    Info.plist
    Program.cs
  Windows/
    app.manifest
    App.xaml
    App.xaml.cs
    Package.appxmanifest
Properties/
  launchSettings.json
Resources/
  AppIcon/
    appicon.svg
    appiconfg.svg
  Fonts/
    OpenSans-Regular.ttf
    OpenSans-Semibold.ttf
  Images/
    dotnet_bot.png
  Raw/
    AboutAssets.txt
  Splash/
    splash.svg
  Styles/
    Colors.xaml
    Styles.xaml
Views/
  RegistroEstudiantesPage.xaml
  RegistroEstudiantesPage.xaml.cs
.gitattributes
.gitignore
App.xaml
App.xaml.cs
MauiProgram.cs
RegistroEstudiantes.csproj
RegistroEstudiantes.slnx
```

# Files

## File: Data/SQLiteDatabase.cs
```csharp
using SQLite;
using RegistroEstudiantes.Models;

namespace RegistroEstudiantes.Data;

/// Maneja la conexión e inicialización de la base de datos SQLite
/// y las operaciones sobre la tabla de productos.
public class SQLiteDatabase
{
    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    private readonly string _dbPath;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SQLiteAsyncConnection? _database;

    public SQLiteDatabase(string dbPath)
    {
        _dbPath = dbPath;
    }

    // Abre la conexión y crea la tabla la primera vez que se necesita,
    // sin bloquear el hilo de la interfaz (sin .Wait()).
    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database is not null)
            return _database;

        await _initLock.WaitAsync();
        try
        {
            if (_database is null)
            {
                var connection = new SQLiteAsyncConnection(_dbPath, Flags);
                await connection.CreateTableAsync<Estudiante>();
                _database = connection;
            }

            return _database;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Create: guarda un producto. Devuelve el número de filas insertadas.
    // Tras insertar, producto.IdProducto contiene el ID generado.
    public async Task<int> GuardarEstudianteAsync(Estudiante estudiante)
    {
        var db = await GetConnectionAsync();
        return await db.InsertAsync(estudiante);
    }

    // Read
    public async Task<List<Estudiante>> ObtenerEstudianteAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<Estudiante>().ToListAsync();
    }

    // Read by Id
    public async Task<Estudiante?> ObtenerEstudianteIdAsync(int id)
    {
        var db = await GetConnectionAsync();
        return await db.FindAsync<Estudiante>(id);
    }

    // Update
    public async Task<int> ActualizarEstudianteAsync(Estudiante estudiante)
    {
        var db = await GetConnectionAsync();
        return await db.UpdateAsync(estudiante);
    }

    // Delete
    public async Task<int> EliminarEstudianteAsync(Estudiante estudiante)
    {
        var db = await GetConnectionAsync();
        return await db.DeleteAsync(estudiante);
    }
}
```

## File: Models/Estudiante.cs
```csharp
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace RegistroEstudiantes.Models
{
    public class Estudiante
    {
        [PrimaryKey, AutoIncrement]
        public int IdEstudiante { get; set; }

        [MaxLength(100), NotNull]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100), NotNull]
        public string Apellido { get; set; } = string.Empty;

        [MaxLength(150), NotNull]
        public string Correo { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Carrera { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
```

## File: Platforms/Android/Resources/values/colors.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <color name="colorPrimary">#512BD4</color>
    <color name="colorPrimaryDark">#2B0B98</color>
    <color name="colorAccent">#2B0B98</color>
</resources>
```

## File: Platforms/Android/AndroidManifest.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
	<application android:allowBackup="true" android:icon="@mipmap/appicon" android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true"></application>
	<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
	<uses-permission android:name="android.permission.INTERNET" />
</manifest>
```

## File: Platforms/Android/MainActivity.cs
```csharp
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace RegistroEstudiantes
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
```

## File: Platforms/Android/MainApplication.cs
```csharp
using Android.App;
using Android.Runtime;

namespace RegistroEstudiantes
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
```

## File: Platforms/iOS/Resources/PrivacyInfo.xcprivacy
```
<?xml version="1.0" encoding="UTF-8"?>
<!--
This is the minimum required version of the Apple Privacy Manifest for .NET MAUI apps.
The contents below are needed because of APIs that are used in the .NET framework and .NET MAUI SDK.

You are responsible for adding extra entries as needed for your application.

More information: https://aka.ms/maui-privacy-manifest
-->
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>NSPrivacyAccessedAPITypes</key>
    <array>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>C617.1</string>
            </array>
        </dict>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategorySystemBootTime</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>35F9.1</string>
            </array>
        </dict>
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryDiskSpace</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>E174.1</string>
            </array>
        </dict>
        <!--
            The entry below is only needed when you're using the Preferences API in your app.
        <dict>
            <key>NSPrivacyAccessedAPIType</key>
            <string>NSPrivacyAccessedAPICategoryUserDefaults</string>
            <key>NSPrivacyAccessedAPITypeReasons</key>
            <array>
                <string>CA92.1</string>
            </array>
        </dict> -->
    </array>
</dict>
</plist>
```

## File: Platforms/iOS/AppDelegate.cs
```csharp
using Foundation;

namespace RegistroEstudiantes
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
```

## File: Platforms/iOS/Info.plist
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>LSRequiresIPhoneOS</key>
	<true/>
	<key>UIDeviceFamily</key>
	<array>
		<integer>1</integer>
		<integer>2</integer>
	</array>
	<key>UIRequiredDeviceCapabilities</key>
	<array>
		<string>arm64</string>
	</array>
	<key>UISupportedInterfaceOrientations</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
	<key>UISupportedInterfaceOrientations~ipad</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationPortraitUpsideDown</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
	<key>XSAppIconAssets</key>
	<string>Assets.xcassets/appicon.appiconset</string>
</dict>
</plist>
```

## File: Platforms/iOS/Program.cs
```csharp
using ObjCRuntime;
using UIKit;

namespace RegistroEstudiantes
{
    public class Program
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
```

## File: Platforms/MacCatalyst/AppDelegate.cs
```csharp
using Foundation;

namespace RegistroEstudiantes
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
```

## File: Platforms/MacCatalyst/Entitlements.plist
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
    <!-- See https://aka.ms/maui-publish-app-store#add-entitlements for more information about adding entitlements.-->
    <dict>
        <!-- App Sandbox must be enabled to distribute a MacCatalyst app through the Mac App Store. -->
        <key>com.apple.security.app-sandbox</key>
        <true/>
        <!-- When App Sandbox is enabled, this value is required to open outgoing network connections. -->
        <key>com.apple.security.network.client</key>
        <true/>
    </dict>
</plist>
```

## File: Platforms/MacCatalyst/Info.plist
```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <!-- The Mac App Store requires you specify if the app uses encryption. -->
    <!-- Please consult https://developer.apple.com/documentation/bundleresources/information_property_list/itsappusesnonexemptencryption -->
    <!-- <key>ITSAppUsesNonExemptEncryption</key> -->
    <!-- Please indicate <true/> or <false/> here. -->

    <!-- Specify the category for your app here. -->
    <!-- Please consult https://developer.apple.com/documentation/bundleresources/information_property_list/lsapplicationcategorytype -->
    <!-- <key>LSApplicationCategoryType</key> -->
    <!-- <string>public.app-category.YOUR-CATEGORY-HERE</string> -->
	<key>UIDeviceFamily</key>
	<array>
		<integer>2</integer>
	</array>
	<key>LSApplicationCategoryType</key>
	<string>public.app-category.lifestyle</string>
	<key>UIRequiredDeviceCapabilities</key>
	<array>
		<string>arm64</string>
	</array>
	<key>UISupportedInterfaceOrientations</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
	<key>UISupportedInterfaceOrientations~ipad</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationPortraitUpsideDown</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
	<key>XSAppIconAssets</key>
	<string>Assets.xcassets/appicon.appiconset</string>
</dict>
</plist>
```

## File: Platforms/MacCatalyst/Program.cs
```csharp
using ObjCRuntime;
using UIKit;

namespace RegistroEstudiantes
{
    public class Program
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
```

## File: Platforms/Windows/app.manifest
```
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <assemblyIdentity version="1.0.0.0" name="RegistroEstudiantes.WinUI.app"/>

  <application xmlns="urn:schemas-microsoft-com:asm.v3">
    <windowsSettings>
      <!-- The combination of below two tags have the following effect:
           1) Per-Monitor for >= Windows 10 Anniversary Update
           2) System < Windows 10 Anniversary Update
      -->
      <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/PM</dpiAware>
      <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2, PerMonitor</dpiAwareness>

      <longPathAware xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">true</longPathAware>
    </windowsSettings>
  </application>
</assembly>
```

## File: Platforms/Windows/App.xaml
```
<maui:MauiWinUIApplication
    x:Class="RegistroEstudiantes.WinUI.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:maui="using:Microsoft.Maui"
    xmlns:local="using:RegistroEstudiantes.WinUI">

</maui:MauiWinUIApplication>
```

## File: Platforms/Windows/App.xaml.cs
```csharp
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RegistroEstudiantes.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
```

## File: Platforms/Windows/Package.appxmanifest
```
<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">

  <Identity Name="maui-package-name-placeholder" Publisher="CN=User Name" Version="0.0.0.0" />

  <mp:PhoneIdentity PhoneProductId="62CA74BA-0895-4822-B25A-0D999F22B280" PhonePublisherId="00000000-0000-0000-0000-000000000000"/>

  <Properties>
    <DisplayName>$placeholder$</DisplayName>
    <PublisherDisplayName>User Name</PublisherDisplayName>
    <Logo>$placeholder$.png</Logo>
  </Properties>

  <Dependencies>
    <TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
  </Dependencies>

  <Resources>
    <Resource Language="x-generate" />
  </Resources>

  <Applications>
    <Application Id="App" Executable="$targetnametoken$.exe" EntryPoint="$targetentrypoint$">
      <uap:VisualElements
        DisplayName="$placeholder$"
        Description="$placeholder$"
        Square150x150Logo="$placeholder$.png"
        Square44x44Logo="$placeholder$.png"
        BackgroundColor="transparent">
        <uap:DefaultTile Square71x71Logo="$placeholder$.png" Wide310x150Logo="$placeholder$.png" Square310x310Logo="$placeholder$.png" />
        <uap:SplashScreen Image="$placeholder$.png" />
      </uap:VisualElements>
    </Application>
  </Applications>

  <Capabilities>
    <rescap:Capability Name="runFullTrust" />
  </Capabilities>

</Package>
```

## File: Properties/launchSettings.json
```json
{
  "profiles": {
    "Windows Machine": {
      "commandName": "Project",
      "nativeDebugging": false
    }
  }
}
```

## File: Resources/AppIcon/appicon.svg
```xml
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg width="456" height="456" viewBox="0 0 456 456" version="1.1" xmlns="http://www.w3.org/2000/svg">
    <rect x="0" y="0" width="456" height="456" fill="#512BD4" />
</svg>
```

## File: Resources/AppIcon/appiconfg.svg
```xml
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN" "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd">
<svg width="456" height="456" viewBox="0 0 456 456" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" xml:space="preserve" xmlns:serif="http://www.serif.com/" style="fill-rule:evenodd;clip-rule:evenodd;stroke-linejoin:round;stroke-miterlimit:2;">
    <path d="m 105.50037,281.60863 c -2.70293,0 -5.00091,-0.90042 -6.893127,-2.70209 -1.892214,-1.84778 -2.837901,-4.04181 -2.837901,-6.58209 0,-2.58722 0.945687,-4.80389 2.837901,-6.65167 1.892217,-1.84778 4.190197,-2.77167 6.893127,-2.77167 2.74819,0 5.06798,0.92389 6.96019,2.77167 1.93749,1.84778 2.90581,4.06445 2.90581,6.65167 0,2.54028 -0.96832,4.73431 -2.90581,6.58209 -1.89221,1.80167 -4.212,2.70209 -6.96019,2.70209 z" style="fill:#ffffff;fill-rule:nonzero;stroke-width:0.838376" />
    <path d="M 213.56111,280.08446 H 195.99044 L 149.69953,207.0544 c -1.17121,-1.84778 -2.14037,-3.76515 -2.90581,-5.75126 h -0.40578 c 0.36051,2.12528 0.54076,6.67515 0.54076,13.6496 v 65.13172 h -15.54349 v -99.36009 h 18.71925 l 44.7374,71.29798 c 1.89222,2.95695 3.1087,4.98917 3.64945,6.09751 h 0.26996 c -0.45021,-2.6325 -0.67573,-7.09015 -0.67573,-13.37293 v -64.02256 h 15.47557 z" style="fill:#ffffff;fill-rule:nonzero;stroke-width:0.838376" />
    <path d="m 289.25134,280.08446 h -54.40052 v -99.36009 h 52.23835 v 13.99669 h -36.15411 v 28.13085 h 33.31621 v 13.9271 h -33.31621 v 29.37835 h 38.31628 z" style="fill:#ffffff;fill-rule:nonzero;stroke-width:0.838376" />
    <path d="M 366.56466,194.72106 H 338.7222 v 85.3634 h -16.08423 v -85.3634 h -27.77455 v -13.99669 h 71.70124 z" style="fill:#ffffff;fill-rule:nonzero;stroke-width:0.838376" />
</svg>
```

## File: Resources/Raw/AboutAssets.txt
```
Any raw assets you want to be deployed with your application can be placed in
this directory (and child directories). Deployment of the asset to your application
is automatically handled by the following `MauiAsset` Build Action within your `.csproj`.

	<MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />

These files will be deployed with your package and will be accessible using Essentials:

	async Task LoadMauiAsset()
	{
		using var stream = await FileSystem.OpenAppPackageFileAsync("AboutAssets.txt");
		using var reader = new StreamReader(stream);

		var contents = reader.ReadToEnd();
	}
```

## File: Resources/Splash/splash.svg
```xml
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN" "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd">
<svg width="456" height="456" viewBox="0 0 456 456" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" xml:space="preserve" xmlns:serif="http://www.serif.com/" style="fill-rule:evenodd;clip-rule:evenodd;stroke-linejoin:round;stroke-miterlimit:2;">
    <path d="m 105.50037,281.60863 c -2.70293,0 -5.00091,-0.90042 -6.893127,-2.70209 -1.892214,-1.84778 -2.837901,-4.04181 -2.837901,-6.58209 0,-2.58722 0.945687,-4.80389 2.837901,-6.65167 1.892217,-1.84778 4.190197,-2.77167 6.893127,-2.77167 2.74819,0 5.06798,0.92389 6.96019,2.77167 1.93749,1.84778 2.90581,4.06445 2.90581,6.65167 0,2.54028 -0.96832,4.73431 -2.90581,6.58209 -1.89221,1.80167 -4.212,2.70209 -6.96019,2.70209 z" style="fill:#ffffff;fill-rule:nonzero;stroke-width:0.838376" />
    <path d="M 213.56111,280.08446 H 195.99044 L 149.69953,207.0544 c -1.17121,-1.84778 -2.14037,-3.76515 -2.90581,-5.75126 h -0.40578 c 0.36051,2.12528 0.54076,6.67515 0.54076,13.6496 v 65.13172 h -15.54349 v -99.36009 h 18.71925 l 44.7374,71.29798 c 1.89222,2.95695 3.1087,4.98917 3.64945,6.09751 h 0.26996 c -0.45021,-2.6325 -0.67573,-7.09015 -0.67573,-13.37293 v -64.02256 h 15.47557 z" style="fill:#ffffff;fill-rule:nonzero;stroke-width:0.838376" />
    <path d="m 289.25134,280.08446 h -54.40052 v -99.36009 h 52.23835 v 13.99669 h -36.15411 v 28.13085 h 33.31621 v 13.9271 h -33.31621 v 29.37835 h 38.31628 z" style="fill:#ffffff;fill-rule:nonzero;stroke-width:0.838376" />
    <path d="M 366.56466,194.72106 H 338.7222 v 85.3634 h -16.08423 v -85.3634 h -27.77455 v -13.99669 h 71.70124 z" style="fill:#ffffff;fill-rule:nonzero;stroke-width:0.838376" />
</svg>
```

## File: Resources/Styles/Colors.xaml
```
<?xml version="1.0" encoding="UTF-8" ?>
<ResourceDictionary 
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Note: For Android please see also Platforms\Android\Resources\values\colors.xml -->

    <Color x:Key="Primary">#512BD4</Color>
    <Color x:Key="PrimaryDark">#ac99ea</Color>
    <Color x:Key="PrimaryDarkText">#242424</Color>
    <Color x:Key="Secondary">#DFD8F7</Color>
    <Color x:Key="SecondaryDarkText">#9880e5</Color>
    <Color x:Key="Tertiary">#2B0B98</Color>

    <Color x:Key="White">White</Color>
    <Color x:Key="Black">Black</Color>
    <Color x:Key="Magenta">#D600AA</Color>
    <Color x:Key="MidnightBlue">#190649</Color>
    <Color x:Key="OffBlack">#1f1f1f</Color>

    <Color x:Key="Gray100">#E1E1E1</Color>
    <Color x:Key="Gray200">#C8C8C8</Color>
    <Color x:Key="Gray300">#ACACAC</Color>
    <Color x:Key="Gray400">#919191</Color>
    <Color x:Key="Gray500">#6E6E6E</Color>
    <Color x:Key="Gray600">#404040</Color>
    <Color x:Key="Gray900">#212121</Color>
    <Color x:Key="Gray950">#141414</Color>

    <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource Primary}"/>
    <SolidColorBrush x:Key="SecondaryBrush" Color="{StaticResource Secondary}"/>
    <SolidColorBrush x:Key="TertiaryBrush" Color="{StaticResource Tertiary}"/>
    <SolidColorBrush x:Key="WhiteBrush" Color="{StaticResource White}"/>
    <SolidColorBrush x:Key="BlackBrush" Color="{StaticResource Black}"/>

    <SolidColorBrush x:Key="Gray100Brush" Color="{StaticResource Gray100}"/>
    <SolidColorBrush x:Key="Gray200Brush" Color="{StaticResource Gray200}"/>
    <SolidColorBrush x:Key="Gray300Brush" Color="{StaticResource Gray300}"/>
    <SolidColorBrush x:Key="Gray400Brush" Color="{StaticResource Gray400}"/>
    <SolidColorBrush x:Key="Gray500Brush" Color="{StaticResource Gray500}"/>
    <SolidColorBrush x:Key="Gray600Brush" Color="{StaticResource Gray600}"/>
    <SolidColorBrush x:Key="Gray900Brush" Color="{StaticResource Gray900}"/>
    <SolidColorBrush x:Key="Gray950Brush" Color="{StaticResource Gray950}"/>
</ResourceDictionary>
```

## File: Resources/Styles/Styles.xaml
```
<?xml version="1.0" encoding="UTF-8" ?>
<ResourceDictionary 
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <Style TargetType="ActivityIndicator">
        <Setter Property="Color" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}" />
    </Style>

    <Style TargetType="IndicatorView">
        <Setter Property="IndicatorColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}"/>
        <Setter Property="SelectedIndicatorColor" Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray100}}"/>
    </Style>

    <Style TargetType="Border">
        <Setter Property="Stroke" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}" />
        <Setter Property="StrokeShape" Value="Rectangle"/>
        <Setter Property="StrokeThickness" Value="1"/>
    </Style>

    <Style TargetType="BoxView">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray200}}" />
    </Style>

    <Style TargetType="Button">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource PrimaryDarkText}}" />
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource PrimaryDark}}" />
        <Setter Property="FontFamily" Value="OpenSansRegular"/>
        <Setter Property="FontSize" Value="14"/>
        <Setter Property="BorderWidth" Value="0"/>
        <Setter Property="CornerRadius" Value="8"/>
        <Setter Property="Padding" Value="14,10"/>
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray200}}" />
                            <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="PointerOver" />
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="CheckBox">
        <Setter Property="Color" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}" />
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="Color" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="DatePicker">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}" />
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="FontFamily" Value="OpenSansRegular"/>
        <Setter Property="FontSize" Value="14"/>
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="Editor">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}" />
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="FontFamily" Value="OpenSansRegular"/>
        <Setter Property="FontSize" Value="14" />
        <Setter Property="PlaceholderColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}" />
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="Entry">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}" />
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="FontFamily" Value="OpenSansRegular"/>
        <Setter Property="FontSize" Value="14" />
        <Setter Property="PlaceholderColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray500}}" />
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="ImageButton">
        <Setter Property="Opacity" Value="1" />
        <Setter Property="BorderColor" Value="Transparent"/>
        <Setter Property="BorderWidth" Value="0"/>
        <Setter Property="CornerRadius" Value="0"/>
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="Opacity" Value="0.5" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="PointerOver" />
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="Label">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}" />
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="FontFamily" Value="OpenSansRegular" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="Label" x:Key="Headline">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource MidnightBlue}, Dark={StaticResource White}}" />
        <Setter Property="FontSize" Value="32" />
        <Setter Property="HorizontalOptions" Value="Center" />
        <Setter Property="HorizontalTextAlignment" Value="Center" />
    </Style>

    <Style TargetType="Label" x:Key="SubHeadline">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource MidnightBlue}, Dark={StaticResource White}}" />
        <Setter Property="FontSize" Value="24" />
        <Setter Property="HorizontalOptions" Value="Center" />
        <Setter Property="HorizontalTextAlignment" Value="Center" />
    </Style>

    <Style TargetType="Picker">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}" />
        <Setter Property="TitleColor" Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource Gray200}}" />
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="FontFamily" Value="OpenSansRegular"/>
        <Setter Property="FontSize" Value="14" />
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                            <Setter Property="TitleColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="ProgressBar">
        <Setter Property="ProgressColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="ProgressColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="RadioButton">
        <Setter Property="BackgroundColor" Value="Transparent"/>
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}" />
        <Setter Property="FontFamily" Value="OpenSansRegular"/>
        <Setter Property="FontSize" Value="14"/>
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="RefreshView">
        <Setter Property="RefreshColor" Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource Gray200}}" />
    </Style>

    <Style TargetType="SearchBar">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}" />
        <Setter Property="PlaceholderColor" Value="{StaticResource Gray500}" />
        <Setter Property="CancelButtonColor" Value="{StaticResource Gray500}" />
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="FontFamily" Value="OpenSansRegular" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                            <Setter Property="PlaceholderColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="SearchHandler">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}" />
        <Setter Property="PlaceholderColor" Value="{StaticResource Gray500}" />
        <Setter Property="BackgroundColor" Value="Transparent" />
        <Setter Property="FontFamily" Value="OpenSansRegular" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                            <Setter Property="PlaceholderColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="Shadow">
        <Setter Property="Radius" Value="15" />
        <Setter Property="Opacity" Value="0.5" />
        <Setter Property="Brush" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource White}}" />
        <Setter Property="Offset" Value="10,10" />
    </Style>

    <Style TargetType="Slider">
        <Setter Property="MinimumTrackColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}" />
        <Setter Property="MaximumTrackColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray600}}" />
        <Setter Property="ThumbColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="MinimumTrackColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                            <Setter Property="MaximumTrackColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                            <Setter Property="ThumbColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}"/>
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="SwipeItem">
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}" />
    </Style>

    <Style TargetType="Switch">
        <Setter Property="OnColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}" />
        <Setter Property="ThumbColor" Value="{StaticResource White}" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="OnColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                            <Setter Property="ThumbColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="On">
                        <VisualState.Setters>
                            <Setter Property="OnColor" Value="{AppThemeBinding Light={StaticResource Secondary}, Dark={StaticResource Gray200}}" />
                            <Setter Property="ThumbColor" Value="{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource White}}" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="Off">
                        <VisualState.Setters>
                            <Setter Property="ThumbColor" Value="{AppThemeBinding Light={StaticResource Gray400}, Dark={StaticResource Gray500}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <Style TargetType="TimePicker">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource White}}" />
        <Setter Property="BackgroundColor" Value="Transparent"/>
        <Setter Property="FontFamily" Value="OpenSansRegular"/>
        <Setter Property="FontSize" Value="14"/>
        <Setter Property="MinimumHeightRequest" Value="44"/>
        <Setter Property="MinimumWidthRequest" Value="44"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Disabled">
                        <VisualState.Setters>
                            <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray600}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
  
    <!--
    <Style TargetType="TitleBar">
        <Setter Property="MinimumHeightRequest" Value="32"/>
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="TitleActiveStates">
                    <VisualState x:Name="TitleBarTitleActive">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="Transparent" />
                            <Setter Property="ForegroundColor" Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource White}}" />
                        </VisualState.Setters>
                    </VisualState>
                    <VisualState x:Name="TitleBarTitleInactive">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}" />
                            <Setter Property="ForegroundColor" Value="{AppThemeBinding Light={StaticResource Gray400}, Dark={StaticResource Gray500}}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>
    -->

    <Style TargetType="Page" ApplyToDerivedTypes="True">
        <Setter Property="Padding" Value="0"/>
        <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource OffBlack}}" />
    </Style>

    <Style TargetType="Shell" ApplyToDerivedTypes="True">
        <Setter Property="Shell.BackgroundColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource OffBlack}}" />
        <Setter Property="Shell.ForegroundColor" Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource SecondaryDarkText}}" />
        <Setter Property="Shell.TitleColor" Value="{AppThemeBinding Light={StaticResource Black}, Dark={StaticResource SecondaryDarkText}}" />
        <Setter Property="Shell.DisabledColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray950}}" />
        <Setter Property="Shell.UnselectedColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray200}}" />
        <Setter Property="Shell.NavBarHasShadow" Value="False" />
        <Setter Property="Shell.TabBarBackgroundColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}" />
        <Setter Property="Shell.TabBarForegroundColor" Value="{AppThemeBinding Light={StaticResource Magenta}, Dark={StaticResource White}}" />
        <Setter Property="Shell.TabBarTitleColor" Value="{AppThemeBinding Light={StaticResource Magenta}, Dark={StaticResource White}}" />
        <Setter Property="Shell.TabBarUnselectedColor" Value="{AppThemeBinding Light={StaticResource Gray900}, Dark={StaticResource Gray200}}" />
    </Style>

    <Style TargetType="NavigationPage">
        <Setter Property="BarBackgroundColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource OffBlack}}" />
        <Setter Property="BarTextColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource White}}" />
        <Setter Property="IconColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource White}}" />
    </Style>

    <Style TargetType="TabbedPage">
        <Setter Property="BarBackgroundColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Gray950}}" />
        <Setter Property="BarTextColor" Value="{AppThemeBinding Light={StaticResource Magenta}, Dark={StaticResource White}}" />
        <Setter Property="UnselectedTabColor" Value="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray950}}" />
        <Setter Property="SelectedTabColor" Value="{AppThemeBinding Light={StaticResource Gray950}, Dark={StaticResource Gray200}}" />
    </Style>

</ResourceDictionary>
```

## File: .gitattributes
```
###############################################################################
# Set default behavior to automatically normalize line endings.
###############################################################################
* text=auto

###############################################################################
# Set default behavior for command prompt diff.
#
# This is need for earlier builds of msysgit that does not have it on by
# default for csharp files.
# Note: This is only used by command line
###############################################################################
#*.cs     diff=csharp

###############################################################################
# Set the merge driver for project and solution files
#
# Merging from the command prompt will add diff markers to the files if there
# are conflicts (Merging from VS is not affected by the settings below, in VS
# the diff markers are never inserted). Diff markers may cause the following 
# file extensions to fail to load in VS. An alternative would be to treat
# these files as binary and thus will always conflict and require user
# intervention with every merge. To do so, just uncomment the entries below
###############################################################################
#*.sln       merge=binary
#*.csproj    merge=binary
#*.vbproj    merge=binary
#*.vcxproj   merge=binary
#*.vcproj    merge=binary
#*.dbproj    merge=binary
#*.fsproj    merge=binary
#*.lsproj    merge=binary
#*.wixproj   merge=binary
#*.modelproj merge=binary
#*.sqlproj   merge=binary
#*.wwaproj   merge=binary

###############################################################################
# behavior for image files
#
# image files are treated as binary by default.
###############################################################################
#*.jpg   binary
#*.png   binary
#*.gif   binary

###############################################################################
# diff behavior for common document formats
# 
# Convert binary document formats to text before diffing them. This feature
# is only available from the command line. Turn it on by uncommenting the 
# entries below.
###############################################################################
#*.doc   diff=astextplain
#*.DOC   diff=astextplain
#*.docx  diff=astextplain
#*.DOCX  diff=astextplain
#*.dot   diff=astextplain
#*.DOT   diff=astextplain
#*.pdf   diff=astextplain
#*.PDF   diff=astextplain
#*.rtf   diff=astextplain
#*.RTF   diff=astextplain
```

## File: .gitignore
```
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.
##
## Get latest from https://github.com/github/gitignore/blob/master/VisualStudio.gitignore

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# User-specific files (MonoDevelop/Xamarin Studio)
*.userprefs

# Mono auto generated files
mono_crash.*

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Oo]ut/
[Ll]og/
[Ll]ogs/

# Visual Studio 2015/2017 cache/options directory
.vs/
# Uncomment if you have tasks that create the project's static files in wwwroot
#wwwroot/

# Visual Studio 2017 auto generated files
Generated\ Files/

# MSTest test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*

# NUnit
*.VisualState.xml
TestResult.xml
nunit-*.xml

# Build Results of an ATL Project
[Dd]ebugPS/
[Rr]eleasePS/
dlldata.c

# Benchmark Results
BenchmarkDotNet.Artifacts/

# .NET Core
project.lock.json
project.fragment.lock.json
artifacts/

# ASP.NET Scaffolding
ScaffoldingReadMe.txt

# StyleCop
StyleCopReport.xml

# Files built by Visual Studio
*_i.c
*_p.c
*_h.h
*.ilk
*.meta
*.obj
*.iobj
*.pch
*.pdb
*.ipdb
*.pgc
*.pgd
*.rsp
*.sbr
*.tlb
*.tli
*.tlh
*.tmp
*.tmp_proj
*_wpftmp.csproj
*.log
*.vspscc
*.vssscc
.builds
*.pidb
*.svclog
*.scc

# Chutzpah Test files
_Chutzpah*

# Visual C++ cache files
ipch/
*.aps
*.ncb
*.opendb
*.opensdf
*.sdf
*.cachefile
*.VC.db
*.VC.VC.opendb

# Visual Studio profiler
*.psess
*.vsp
*.vspx
*.sap

# Visual Studio Trace Files
*.e2e

# TFS 2012 Local Workspace
$tf/

# Guidance Automation Toolkit
*.gpState

# ReSharper is a .NET coding add-in
_ReSharper*/
*.[Rr]e[Ss]harper
*.DotSettings.user

# TeamCity is a build add-in
_TeamCity*

# DotCover is a Code Coverage Tool
*.dotCover

# AxoCover is a Code Coverage Tool
.axoCover/*
!.axoCover/settings.json

# Coverlet is a free, cross platform Code Coverage Tool
coverage*.json
coverage*.xml
coverage*.info

# Visual Studio code coverage results
*.coverage
*.coveragexml

# NCrunch
_NCrunch_*
.*crunch*.local.xml
nCrunchTemp_*

# MightyMoose
*.mm.*
AutoTest.Net/

# Web workbench (sass)
.sass-cache/

# Installshield output folder
[Ee]xpress/

# DocProject is a documentation generator add-in
DocProject/buildhelp/
DocProject/Help/*.HxT
DocProject/Help/*.HxC
DocProject/Help/*.hhc
DocProject/Help/*.hhk
DocProject/Help/*.hhp
DocProject/Help/Html2
DocProject/Help/html

# Click-Once directory
publish/

# Publish Web Output
*.[Pp]ublish.xml
*.azurePubxml
# Note: Comment the next line if you want to checkin your web deploy settings,
# but database connection strings (with potential passwords) will be unencrypted
*.pubxml
*.publishproj

# Microsoft Azure Web App publish settings. Comment the next line if you want to
# checkin your Azure Web App publish settings, but sensitive information contained
# in these scripts will be unencrypted
PublishScripts/

# NuGet Packages
*.nupkg
# NuGet Symbol Packages
*.snupkg
# The packages folder can be ignored because of Package Restore
**/[Pp]ackages/*
# except build/, which is used as an MSBuild target.
!**/[Pp]ackages/build/
# Uncomment if necessary however generally it will be regenerated when needed
#!**/[Pp]ackages/repositories.config
# NuGet v3's project.json files produces more ignorable files
*.nuget.props
*.nuget.targets

# Microsoft Azure Build Output
csx/
*.build.csdef

# Microsoft Azure Emulator
ecf/
rcf/

# Windows Store app package directories and files
AppPackages/
BundleArtifacts/
Package.StoreAssociation.xml
_pkginfo.txt
*.appx
*.appxbundle
*.appxupload

# Visual Studio cache files
# files ending in .cache can be ignored
*.[Cc]ache
# but keep track of directories ending in .cache
!?*.[Cc]ache/

# Others
ClientBin/
~$*
*~
*.dbmdl
*.dbproj.schemaview
*.jfm
*.pfx
*.publishsettings
orleans.codegen.cs

# Including strong name files can present a security risk
# (https://github.com/github/gitignore/pull/2483#issue-259490424)
#*.snk

# Since there are multiple workflows, uncomment next line to ignore bower_components
# (https://github.com/github/gitignore/pull/1529#issuecomment-104372622)
#bower_components/

# RIA/Silverlight projects
Generated_Code/

# Backup & report files from converting an old project file
# to a newer Visual Studio version. Backup files are not needed,
# because we have git ;-)
_UpgradeReport_Files/
Backup*/
UpgradeLog*.XML
UpgradeLog*.htm
ServiceFabricBackup/
*.rptproj.bak

# SQL Server files
*.mdf
*.ldf
*.ndf

# Business Intelligence projects
*.rdl.data
*.bim.layout
*.bim_*.settings
*.rptproj.rsuser
*- [Bb]ackup.rdl
*- [Bb]ackup ([0-9]).rdl
*- [Bb]ackup ([0-9][0-9]).rdl

# Microsoft Fakes
FakesAssemblies/

# GhostDoc plugin setting file
*.GhostDoc.xml

# Node.js Tools for Visual Studio
.ntvs_analysis.dat
node_modules/

# Visual Studio 6 build log
*.plg

# Visual Studio 6 workspace options file
*.opt

# Visual Studio 6 auto-generated workspace file (contains which files were open etc.)
*.vbw

# Visual Studio LightSwitch build output
**/*.HTMLClient/GeneratedArtifacts
**/*.DesktopClient/GeneratedArtifacts
**/*.DesktopClient/ModelManifest.xml
**/*.Server/GeneratedArtifacts
**/*.Server/ModelManifest.xml
_Pvt_Extensions

# Paket dependency manager
.paket/paket.exe
paket-files/

# FAKE - F# Make
.fake/

# CodeRush personal settings
.cr/personal

# Python Tools for Visual Studio (PTVS)
__pycache__/
*.pyc

# Cake - Uncomment if you are using it
# tools/**
# !tools/packages.config

# Tabs Studio
*.tss

# Telerik's JustMock configuration file
*.jmconfig

# BizTalk build output
*.btp.cs
*.btm.cs
*.odx.cs
*.xsd.cs

# OpenCover UI analysis results
OpenCover/

# Azure Stream Analytics local run output
ASALocalRun/

# MSBuild Binary and Structured Log
*.binlog

# NVidia Nsight GPU debugger configuration file
*.nvuser

# MFractors (Xamarin productivity tool) working folder
.mfractor/

# Local History for Visual Studio
.localhistory/

# BeatPulse healthcheck temp database
healthchecksdb

# Backup folder for Package Reference Convert tool in Visual Studio 2017
MigrationBackup/

# Ionide (cross platform F# VS Code tools) working folder
.ionide/

# Fody - auto-generated XML schema
FodyWeavers.xsd
```

## File: App.xaml
```
<?xml version = "1.0" encoding = "UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:RegistroEstudiantes"
             x:Class="RegistroEstudiantes.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## File: App.xaml.cs
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace RegistroEstudiantes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new Views.RegistroEstudiantesPage()));
        }
    }
}
```

## File: MauiProgram.cs
```csharp
using Microsoft.Extensions.Logging;

namespace RegistroEstudiantes
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
```

## File: RegistroEstudiantes.csproj
```
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFrameworks>net10.0-android</TargetFrameworks>
		<TargetFrameworks Condition="!$([MSBuild]::IsOSPlatform('linux'))">$(TargetFrameworks);net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
		<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>

		<!-- Note for MacCatalyst:
		The default runtime is maccatalyst-x64, except in Release config, in which case the default is maccatalyst-x64;maccatalyst-arm64.
		When specifying both architectures, use the plural <RuntimeIdentifiers> instead of the singular <RuntimeIdentifier>.
		The Mac App Store will NOT accept apps with ONLY maccatalyst-arm64 indicated;
		either BOTH runtimes must be indicated or ONLY macatalyst-x64. -->
		<!-- For example: <RuntimeIdentifiers>maccatalyst-x64;maccatalyst-arm64</RuntimeIdentifiers> -->

		<OutputType>Exe</OutputType>
		<RootNamespace>RegistroEstudiantes</RootNamespace>
		<UseMaui>true</UseMaui>
		<SingleProject>true</SingleProject>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>

		<!-- Enable XAML source generation for faster build times and improved performance.
		     This generates C# code from XAML at compile time instead of runtime inflation.
		     To disable, remove this line.
		     For individual files, you can override by setting Inflator metadata:
		       <MauiXaml Update="MyPage.xaml" Inflator="Default" /> (reverts to defaults: Runtime for Debug, XamlC for Release)
		       <MauiXaml Update="MyPage.xaml" Inflator="Runtime" /> (force runtime inflation) -->
		<MauiXamlInflator>SourceGen</MauiXamlInflator>

		<!-- Display name -->
		<ApplicationTitle>RegistroEstudiantes</ApplicationTitle>

		<!-- App Identifier -->
		<ApplicationId>com.companyname.registroestudiantes</ApplicationId>

		<!-- Versions -->
		<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
		<ApplicationVersion>1</ApplicationVersion>

		<!-- To develop, package, and publish an app to the Microsoft Store, see: https://aka.ms/MauiTemplateUnpackaged -->
		<WindowsPackageType>None</WindowsPackageType>

		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">15.0</SupportedOSPlatformVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">15.0</SupportedOSPlatformVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</SupportedOSPlatformVersion>
		<TargetPlatformMinVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</TargetPlatformMinVersion>
	</PropertyGroup>

	<ItemGroup>
		<!-- App Icon -->
		<MauiIcon Include="Resources\AppIcon\appicon.svg" ForegroundFile="Resources\AppIcon\appiconfg.svg" Color="#512BD4" />

		<!-- Splash Screen -->
		<MauiSplashScreen Include="Resources\Splash\splash.svg" Color="#512BD4" BaseSize="128,128" />

		<!-- Images -->
		<MauiImage Include="Resources\Images\*" />
		<MauiImage Update="Resources\Images\dotnet_bot.png" Resize="True" BaseSize="300,185" />
		
		<!-- Custom Fonts -->
		<MauiFont Include="Resources\Fonts\*" />

		<!-- Raw Assets (also remove the "Resources\Raw" prefix) -->
		<MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
	</ItemGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
		<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.0" />
		<PackageReference Include="sqlite-net-sqlcipher" Version="1.9.172" />
	</ItemGroup>

	<ItemGroup>
	  <MauiXaml Update="Views\RegistroEstudiantesPage.xaml">
	    <Generator>MSBuild:Compile</Generator>
	  </MauiXaml>
	</ItemGroup>

</Project>
```

## File: RegistroEstudiantes.slnx
```
<Solution>
  <Project Path="RegistroEstudiantes.csproj" />
</Solution>
```

## File: Views/RegistroEstudiantesPage.xaml
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="RegistroEstudiantes.Views.RegistroEstudiantesPage"
             Title="Registro de Estudiantes">
    <ScrollView>
        <VerticalStackLayout
            Padding="30"
            Spacing="10">

            <Label
                Text="Registro de Estudiantes"
                Style="{StaticResource Headline}"
                SemanticProperties.HeadingLevel="Level1"
                FontAttributes="Bold"
                HorizontalOptions="Center"
                FontSize="28" />

            <!-- ID -->
            <Label Text="ID Estudiante" />
            <Entry
                x:Name="IdEntry"
                Placeholder="Se genera automáticamente"
                IsEnabled="False"
                SemanticProperties.Description="ID del estudiante, se genera automáticamente" />

            <!-- Nombre -->
            <Label Text="Nombre *" />
            <Entry
                x:Name="NombreEntry"
                Placeholder="Nombre del estudiante"
                Keyboard="Text"
                MaxLength="100"
                ClearButtonVisibility="WhileEditing"
                SemanticProperties.Description="Nombre del estudiante" />

            <!-- Apellido -->
            <Label Text="Apellido *" />
            <Entry
                x:Name="ApellidoEntry"
                Placeholder="Apellido del estudiante"
                Keyboard="Text"
                MaxLength="100"
                ClearButtonVisibility="WhileEditing"
                SemanticProperties.Description="Apellido del estudiante" />

            <!-- Correo Electrónico -->
            <Label Text="Correo Electrónico *" />
            <Entry
                x:Name="CorreoEntry"
                Placeholder="ejemplo@correo.com"
                Keyboard="Email"
                MaxLength="150"
                ClearButtonVisibility="WhileEditing"
                SemanticProperties.Description="Correo electrónico del estudiante" />

            <!-- Teléfono -->
            <Label Text="Teléfono" />
            <Entry
                x:Name="TelefonoEntry"
                Placeholder="Número de teléfono"
                Keyboard="Telephone"
                MaxLength="20"
                ClearButtonVisibility="WhileEditing"
                SemanticProperties.Description="Teléfono del estudiante" />

            <!-- Carrera -->
            <Label Text="Carrera / Grado" />
            <Entry
                x:Name="CarreraEntry"
                Placeholder="Ingeniería en Sistemas, Administración, etc."
                Keyboard="Text"
                MaxLength="100"
                ClearButtonVisibility="WhileEditing"
                SemanticProperties.Description="Carrera del estudiante" />

            <!-- Fecha de Registro -->
            <Label Text="Fecha de Registro *" />
            <DatePicker
                x:Name="FechaRegistroPicker"
                SemanticProperties.Description="Fecha de registro del estudiante" />

            <!-- Botón Guardar -->
            <Button
                x:Name="GuardarButton"
                Text="Guardar Estudiante"
                Margin="0,15,0,0"
                SemanticProperties.Description="Guarda la información del estudiante"
                Clicked="GuardarButton_Clicked" />

        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

## File: Views/RegistroEstudiantesPage.xaml.cs
```csharp
using System.Globalization;
using RegistroEstudiantes.Data;
using RegistroEstudiantes.Models;

namespace RegistroEstudiantes.Views;

public partial class RegistroEstudiantesPage : ContentPage
{
    private readonly SQLiteDatabase _database;

    public RegistroEstudiantesPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "estudiantes.db3");
        _database = new SQLiteDatabase(dbPath);

        LimpiarFormulario();
    }

    private async void GuardarButton_Clicked(object? sender, EventArgs e)
    {
        // 1. Validar los datos ingresados
        Estudiante? estudiante = ValidarFormulario(out List<string> errores, out VisualElement? campoInvalido);

        if (estudiante is null)
        {
            await DisplayAlert(
                "Datos incompletos o inválidos",
                string.Join(Environment.NewLine, errores),
                "OK");

            campoInvalido?.Focus();
            return;
        }

        // 2. Guardar en SQLite (deshabilitamos el botón para evitar duplicados por doble toque)
        GuardarButton.IsEnabled = false;
        try
        {
            await _database.GuardarEstudianteAsync(estudiante);

            await DisplayAlert(
                "Éxito",
                $"El estudiante se registró correctamente (ID: {estudiante.IdEstudiante}).",
                "OK");

            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                $"Ocurrió un error al guardar el estudiante: {ex.Message}",
                "OK");
        }
        finally
        {
            GuardarButton.IsEnabled = true;
        }
    }

    /// Valida todos los campos. Devuelve el estudiante si todo es correcto;
    /// de lo contrario devuelve null, la lista de errores y el primer campo con error.
    private Estudiante? ValidarFormulario(out List<string> errores, out VisualElement? primerCampoInvalido)
    {
        var listaErrores = new List<string>();
        VisualElement? primerInvalido = null;

        void Falla(string mensaje, VisualElement campo)
        {
            listaErrores.Add($"• {mensaje}");
            primerInvalido ??= campo;
        }

        // Nombre
        string nombre = NombreEntry.Text?.Trim() ?? string.Empty;
        if (nombre.Length == 0)
            Falla("El nombre del estudiante es obligatorio.", NombreEntry);

        // Apellido
        string apellido = ApellidoEntry.Text?.Trim() ?? string.Empty;
        if (apellido.Length == 0)
            Falla("El apellido del estudiante es obligatorio.", ApellidoEntry);

        // Correo
        string correo = CorreoEntry.Text?.Trim() ?? string.Empty;
        if (correo.Length == 0)
            Falla("El correo electrónico es obligatorio.", CorreoEntry);
        else if (!correo.Contains('@') || !correo.Contains('.'))
            Falla("El correo electrónico no es válido.", CorreoEntry);

        // Teléfono (opcional, pero si se llena validamos longitud o formato opcional)
        string telefono = TelefonoEntry.Text?.Trim() ?? string.Empty;

        // Carrera (opcional)
        string carrera = CarreraEntry.Text?.Trim() ?? string.Empty;

        // Fecha de registro
        DateTime fecha = DateTime.Today;
        if (FechaRegistroPicker.Date is not DateTime fechaSeleccionada)
            Falla("Selecciona la fecha de registro.", FechaRegistroPicker);
        else if (fechaSeleccionada.Date > DateTime.Today)
            Falla("La fecha de registro no puede ser futura.", FechaRegistroPicker);
        else
            fecha = fechaSeleccionada.Date;

        errores = listaErrores;
        primerCampoInvalido = primerInvalido;

        if (listaErrores.Count > 0)
            return null;

        return new Estudiante
        {
            Nombre = nombre,
            Apellido = apellido,
            Correo = correo,
            Telefono = telefono,
            Carrera = carrera,
            FechaRegistro = fecha
        };
    }

    private void LimpiarFormulario()
    {
        IdEntry.Text = string.Empty;
        NombreEntry.Text = string.Empty;
        ApellidoEntry.Text = string.Empty;
        CorreoEntry.Text = string.Empty;
        TelefonoEntry.Text = string.Empty;
        CarreraEntry.Text = string.Empty;
        FechaRegistroPicker.Date = DateTime.Today;
    }
}
```
