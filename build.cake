#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=gitreleasemanager"
using System.Collections.Generic;
using static System.Text.RegularExpressions.Regex;

#region Fields
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = Argument<string>("appversion", null);
var tag = Argument<string>("apptag", null);
var chocoVersion = tag?.Substring(1);
var prerelease = false;

// Deploy on Release Tag builds
var deploy = configuration == "Release" && !string.IsNullOrWhiteSpace(tag);

const string slnPath = "src/Captura.sln";

public class Backup : System.IDisposable
{
    readonly ICakeContext _context;

    public static List<Backup> Backups { get; } = new List<Backup>();

    public Backup(ICakeContext Context, string OriginalPath, string BackupPath)
    {
        _context = Context;

        this.OriginalPath = OriginalPath;
        this.BackupPath = BackupPath;

        _context.CopyFile(OriginalPath, BackupPath);
    }

    public string OriginalPath { get; }

    public string BackupPath { get; }

    public void Restore()
    {
        _context.DeleteFile(OriginalPath);
        _context.MoveFile(BackupPath, OriginalPath);
    }

    public void Dispose()
    {
        Restore();
    }
}

void CreateBackup(string OriginalPath, string BackupPath)
{
    var backup = new Backup(Context, OriginalPath, BackupPath);
    Backup.Backups.Add(backup);
}
#endregion

#region Functions
void HandleTag()
{
    // Not a Tag Build
    if (string.IsNullOrWhiteSpace(tag))
        return;

    // Stable Release
    if (IsMatch(tag, @"^v\d+\.\d+\.\d+$"))
    {
        version = tag;
    }
    // Prerelease
    else if (IsMatch(tag, @"^v\d+\.\d+\.\d+-[^\s]+$"))
    {
        prerelease = true;

        version = tag.Split('-')[0];
    }
    else throw new System.ArgumentException("Invalid Tag Format", "Tag");
}

void UpdateVersion(string AssemblyInfoPath)
{
    var content = FileRead(AssemblyInfoPath);

    var start = content.IndexOf("AssemblyVersion");
    var end = content.IndexOf(")", start);

    var replace = content.Replace(content.Substring(start, end - start + 1), $"AssemblyVersion(\"{version}\")");

    FileWrite(AssemblyInfoPath, replace);
}

string FileRead(string FileName) => System.IO.File.ReadAllText(FileName);

void FileWrite(string FileName, string Content) => System.IO.File.WriteAllText(FileName, Content);

void HandleVersion()
{
    const string uiAssemblyInfo = "src/Captura/Properties/AssemblyInfo.cs";
    const string consoleAssemblyInfo = "src/Captura.Console/Properties/AssemblyInfo.cs";

    if (string.IsNullOrWhiteSpace(version))
    {
        // Read from AssemblyInfo
        version = ParseAssemblyInfo(uiAssemblyInfo).AssemblyVersion;
    }
    else
    {
        // Update AssemblyInfo files
        CreateBackup(uiAssemblyInfo, "temp/AssemblyInfo.cs");
        CreateBackup(consoleAssemblyInfo, "temp/console.cs");

        UpdateVersion(uiAssemblyInfo);
        UpdateVersion(consoleAssemblyInfo);
    }
}

void EmbedApiKeys()
{
    const string apiKeysPath = "src/Captura.Core/ApiKeys.cs";
    const string imgurEnv = "imgur_client_id";

    // Embed Api Keys in Release builds
    if (configuration == "Release" && HasEnvironmentVariable(imgurEnv))
    {
        Information("Embedding Api Keys from Environment Variables ...");

        CreateBackup(apiKeysPath, "temp/ApiKeys.cs");

        var apiKeysOriginalContent = FileRead(apiKeysPath);

        var newContent = apiKeysOriginalContent.Replace($"Get(\"{imgurEnv}\")", $"\"{EnvironmentVariable(imgurEnv)}\"");

        FileWrite(apiKeysPath, newContent);
    }
}

// Restores native dlls
void NativeRestore()
{
    var bass = "temp/bass/bass.dll";
    var bassmix = "temp/bassmix/bassmix.dll";

    Information("Restoring Native libraries...");

    if (!FileExists(bass))
    {
        Information("Downloading BASS...");

        DownloadFile("http://www.un4seen.com/files/bass24.zip", "temp/bass.zip");

        Information("Extracting BASS ...");

        Unzip("temp/bass.zip", "temp/bass");
    }

    if (!FileExists(bassmix))
    {
        Information("Downloading BASSmix...");

        DownloadFile("http://www.un4seen.com/files/bassmix24.zip", "temp/bassmix.zip");

        Information("Extracting BASSmix...");

        Unzip("temp/bassmix.zip", "temp/bassmix");
    }

    var consoleOutput = $"src/Captura.Console/bin/{configuration}/";
    var uiOutput = $"src/Captura/bin/{configuration}/";
    var testOutput = $"src/Tests/bin/{configuration}/";

    foreach (var output in new[] { consoleOutput, uiOutput, testOutput })
    {
        var path = output;

        if (configuration != "Debug")
        {
            path += "/lib/";
        }

        EnsureDirectoryExists(path);

        CopyFileToDirectory(bass, path);
        CopyFileToDirectory(bassmix, path);
    }
}

void CopyLicenses()
{
    var consoleOutput = $"src/Captura.Console/bin/{configuration}/";
    var uiOutput = $"src/Captura/bin/{configuration}/";
    var testOutput = $"src/Tests/bin/{configuration}/";

    foreach (var output in new[] { consoleOutput, uiOutput, testOutput })
    {
        CopyDirectory("licenses", output + "licenses");
    }
}

void PopulateOutput()
{
    // Copy License files
    CopyDirectory("licenses", "dist/licenses");

    var consoleBinFolder = $"src/Captura.Console/bin/{configuration}/";
    var uiBinFolder = $"src/Captura/bin/{configuration}/";
    
    // Copy Languages
    CopyDirectory(uiBinFolder + "Languages", "dist/Languages");

    // Copy executables and config files
    CopyFiles(consoleBinFolder + "*.exe*", "dist");
    CopyFiles(uiBinFolder + "*.exe*", "dist");

    // For Debug builds
    if (configuration == "Debug")
    {
        // Copy Assemblies
        CopyFiles(consoleBinFolder + "*.dll", "dist");
        CopyFiles(uiBinFolder + "*.dll", "dist");

        // Copy symbol files
        CopyFiles(consoleBinFolder + "*.pdb", "dist");
        CopyFiles(uiBinFolder + "*.pdb", "dist");

        // Copy Xml Documentation
        CopyFiles(consoleBinFolder + "*.xml", "dist");
        CopyFiles(uiBinFolder + "*.xml", "dist");
    }
    else
    {
        CopyDirectory(consoleBinFolder + "lib", "dist/lib");
        CopyDirectory(uiBinFolder + "lib", "dist/lib");
    }
}

void PackChoco(string Tag, string Version)
{
    var checksum = CalculateFileHash("temp/Captura-Portable.zip").ToHex();

    var chocoInstallScript = "choco/tools/chocolateyinstall.ps1";

    var originalContent = FileRead(chocoInstallScript);

    var newContent = $"$tag = '{Tag}'; $checksum = '{checksum}'; {originalContent}";

    using (var backup = new Backup(Context, chocoInstallScript, "temp/cinst.ps1"))
    {
        FileWrite(chocoInstallScript, newContent);

        ChocolateyPack("choco/captura.nuspec", new ChocolateyPackSettings
        {
            Version = Version,
            ArgumentCustomization = Args => Args.Append("--outputdirectory temp")
        });
    }
}
#endregion

#region Setup / Teardown
Setup(context =>
{
    EnsureDirectoryExists("temp");
    EnsureDirectoryExists("dist");

    HandleTag();

    HandleVersion();

    EmbedApiKeys();
});

Teardown(context =>
{
    Backup.Backups.ForEach(M => M.Restore());
});
#endregion

#region Tasks
var cleanTask = Task("Clean").Does(() =>
{
    MSBuild(slnPath, settings =>
    {
        settings.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("Clean");
    });
});

var nugetRestoreTask = Task("Nuget-Restore").Does(() => NuGetRestore(slnPath));

var buildTask = Task("Build")
    .IsDependentOn(nugetRestoreTask)
    .Does(() =>
{
    MSBuild(slnPath, settings =>
    {
        settings.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("Rebuild");
    });

    NativeRestore();

    CopyLicenses();
});

var cleanOutputTask = Task("Clean-Output").Does(() => CleanDirectory("dist"));

var populateOutputTask = Task("Populate-Output")
    .IsDependentOn(cleanOutputTask)
    .IsDependentOn(buildTask)
    .Does(() => PopulateOutput());

var packPortableTask = Task("Pack-Portable")
    .IsDependentOn(populateOutputTask)
    .Does(() => Zip("dist", "temp/Captura-Portable.zip"));

var packSetupTask = Task("Pack-Setup")
    .WithCriteria(configuration == "Release")
    .IsDependentOn(populateOutputTask)
    .Does(() =>
{
    InnoSetup("Inno.iss", new InnoSetupSettings
    {
        QuietMode = InnoSetupQuietMode.Quiet,
        ArgumentCustomization = Args => Args.Append($"/DMyAppVersion={version}")
    });
});

var packChocoTask = Task("Pack-Choco")
    .WithCriteria(deploy)
    .IsDependentOn(packPortableTask)
    .Does(() => PackChoco(tag, chocoVersion));

var deployGitHubTask = Task("Deploy-GitHub")
    .WithCriteria(deploy)
    .IsDependentOn(packPortableTask)
    .IsDependentOn(packSetupTask)
    .Does(() => 
{
    // Description: [Changelog](https://mathewsachin.github.io/Captura/changelog)
    GitReleaseManagerCreate("MathewSachin",
        EnvironmentVariable("git_key"),
        "MathewSachin",
        "Captura",
        new GitReleaseManagerCreateSettings
        {
            Name = $"Captura {tag}",
            Prerelease = prerelease,
            Assets = "temp/Captura-Portable.zip,temp/Captura-Setup.exe"
        });

    GitReleaseManagerPublish("MathewSachin",
        EnvironmentVariable("git_key"),
        "MathewSachin",
        "Captura",
        tag);
});

var deployChocoTask = Task("Deploy-Choco")
    .WithCriteria(deploy)
    .IsDependentOn(packChocoTask)
    .IsDependentOn(deployGitHubTask)
    .Does(() =>
{
    ChocolateyPush($"temp/captura.{chocoVersion}.nupkg", new ChocolateyPushSettings
    {
        ApiKey = EnvironmentVariable("choco_key")
    });
});

var testTask = Task("Test")
    .IsDependentOn(buildTask)
    .Does(() => XUnit2($"src/Tests/bin/{configuration}/Captura.Tests.dll"));

var defaultTask = Task("Default").IsDependentOn(populateOutputTask);

var installInnoTask = Task("Install-Inno")
    .Does(() => ChocolateyInstall("innosetup", new ChocolateyInstallSettings
    {
        ArgumentCustomization = Args => Args.Append("--no-progress")
    }));
#endregion

#region AppVeyor
class Artifact
{
    public Artifact(string Name, string Path)
    {
        this.Name = Name;
        this.Path = Path;
    }

    public string Name { get; }
    public string Path { get; }
}

var artifacts = new []
{
    new Artifact("Portable", "temp/Captura-Portable.zip"),
    new Artifact("Setup", "temp/Captura-Setup.exe"),
    new Artifact("Chocolatey", $"temp/captura.{chocoVersion}.nupkg")
};

Task("CI")
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor)
    .IsDependentOn(testTask)
    .IsDependentOn(packPortableTask)
    .IsDependentOn(installInnoTask)
    .IsDependentOn(packSetupTask)
    .IsDependentOn(packChocoTask)
    .IsDependentOn(deployGitHubTask)
    .IsDependentOn(deployChocoTask)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion($"{version}.{EnvironmentVariable("APPVEYOR_BUILD_NUMBER")}");

    foreach (var artifact in artifacts)
    {
        if (FileExists(artifact.Path))
        {
            AppVeyor.UploadArtifact(artifact.Path, new AppVeyorUploadArtifactsSettings
            {
                DeploymentName = artifact.Name
            });
        }
    }
});
#endregion

// Start
RunTarget(target);