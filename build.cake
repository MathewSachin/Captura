#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=gitreleasemanager"
#l "tools/scripts/backup.cake"
#l "tools/scripts/constants.cake"
#l "tools/scripts/bass.cake"
#l "tools/scripts/choco.cake"
#l "tools/scripts/apikeys.cake"
#l "tools/scripts/version.cake"
using System.Collections.Generic;

#region Fields
readonly var target = Argument("target", "Default");
readonly var configuration = Argument("configuration", Release);

// Deploy on Release Tag builds
var deploy = configuration == Release && !string.IsNullOrWhiteSpace(tag);
#endregion

#region Functions
IEnumerable<ConvertableDirectoryPath> EnumerateOutputFolders()
{
    var outputProjectNames = new[] { "Captura.Console", "Captura", "Tests" };

    foreach (var output in outputProjectNames)
    {
        yield return sourceFolder + Directory(output) + Directory("bin") + Directory(configuration);
    }
}

// Restores native dlls
void NativeRestore()
{
    Information("Restoring Native libraries...");

    var bass = RestoreBass();
    var bassmix = RestoreBassMix();

    foreach (var output in EnumerateOutputFolders())
    {
        var path = output;

        if (configuration == Release)
        {
            path += Directory("lib");
        }

        EnsureDirectoryExists(path);

        CopyFileToDirectory(bass, path);
        CopyFileToDirectory(bassmix, path);
    }
}

void CopyLicenses()
{
    foreach (var output in EnumerateOutputFolders())
    {
        var path = output + Directory("licenses");

        CopyDirectory(licensesFolder, path);
    }
}

void PopulateOutput()
{
    // Copy License files
    CopyDirectory(licensesFolder, distFolder + Directory("licenses"));

    var consoleBinFolder = sourceFolder + Directory("Captura.Console/bin") + Directory(configuration);
    var uiBinFolder = sourceFolder + Directory("Captura/bin") + Directory(configuration);
    
    // Copy Languages
    CopyDirectory(uiBinFolder + Directory("Languages"), distFolder + Directory("languages"));

    // Copy executables and config files
    CopyFiles(consoleBinFolder.Path + "/*.exe*", distFolder);
    CopyFiles(uiBinFolder.Path + "/*.exe*", distFolder);

    // Copy Keymap files
    CopyDirectory(uiBinFolder + Directory("keymaps"), distFolder + Directory("keymaps"));

    // For Debug builds
    if (configuration != Release)
    {
        // Assemblies, Symbol Files and XML Documentation
        foreach (var extension in new [] { ".dll", ".pdb", ".xml" })
        {
            CopyFiles(consoleBinFolder.Path + "/*" + extension, distFolder);
            CopyFiles(uiBinFolder.Path + "/*" + extension, distFolder);
        }
    }
    else
    {
        CopyDirectory(consoleBinFolder + Directory("lib"), distFolder + Directory("lib"));
        CopyDirectory(uiBinFolder + Directory("lib"), distFolder + Directory("lib"));
    }
}
#endregion

#region Setup / Teardown
Setup(context =>
{
    EnsureDirectoryExists(tempFolder);
    EnsureDirectoryExists(distFolder);

    HandleTag();

    HandleVersion();

    if (configuration == Release)
    {
        EmbedApiKeys();
    }
});

Teardown(context =>
{
    RestoreBackups();
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

var cleanOutputTask = Task("Clean-Output").Does(() => CleanDirectory(distFolder));

var populateOutputTask = Task("Populate-Output")
    .IsDependentOn(cleanOutputTask)
    .IsDependentOn(buildTask)
    .Does(() => PopulateOutput());

var packPortableTask = Task("Pack-Portable")
    .IsDependentOn(populateOutputTask)
    .Does(() => Zip(distFolder, PortablePath));

var packSetupTask = Task("Pack-Setup")
    .WithCriteria(configuration == Release)
    .IsDependentOn(populateOutputTask)
    .Does(() =>
{
    const string InnoScriptPath = "Inno.iss";

    InnoSetup(InnoScriptPath, new InnoSetupSettings
    {
        QuietMode = InnoSetupQuietMode.Quiet,
        ArgumentCustomization = Args => Args.Append($"/DMyAppVersion={version}")
    });
});

var deployGitHubTask = Task("Deploy-GitHub")
    .WithCriteria(deploy)
    .IsDependentOn(packPortableTask)
    .IsDependentOn(packSetupTask)
    .Does(() => 
{
    var releaseNotesPath = tempFolder + File("release_notes.md");
    const string changelogUrl = "https://mathewsachin.github.io/Captura/changelog";

    FileWrite(releaseNotesPath, $"[Changelog]({changelogUrl})");

    const string RepoOwner = "MathewSachin";
    const string RepoName = "Captura";

    GitReleaseManagerCreate(RepoOwner,
        EnvironmentVariable("git_key"),
        RepoOwner,
        RepoName,
        new GitReleaseManagerCreateSettings
        {
            Name = $"Captura {tag}",
            InputFilePath = releaseNotesPath,
            Prerelease = prerelease,
            Assets = $"{PortablePath},{SetupPath}"
        });

    GitReleaseManagerPublish(RepoOwner,
        EnvironmentVariable("git_key"),
        RepoOwner,
        RepoName,
        tag);
});

var packChocoTask = Task("Pack-Choco")
    .WithCriteria(deploy)
    .IsDependentOn(packPortableTask)
    .IsDependentOn(deployGitHubTask)
    .Does(() => PackChoco());

var deployChocoTask = Task("Deploy-Choco")
    .WithCriteria(deploy)
    .IsDependentOn(packChocoTask)
    .IsDependentOn(deployGitHubTask)
    .Does(() => PushChoco());

var testTask = Task("Test")
    .IsDependentOn(buildTask)
    .Does(() => XUnit2(sourceFolder + File($"Tests/bin/{configuration}/Captura.Tests.dll")));

var defaultTask = Task("Default").IsDependentOn(populateOutputTask);

var installInnoTask = Task("Install-Inno")
    .WithCriteria(configuration == Release)
    .Does(() => ChocolateyInstall("innosetup", new ChocolateyInstallSettings
    {
        ArgumentCustomization = Args => Args.Append("--no-progress")
    }));
#endregion

#region AppVeyor
#l "tools/scripts/appveyor.cake"

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
    if (deploy)
    {
        AppVeyor.UpdateBuildVersion($"{version}.{buildNo}");
    }

    UploadArtifacts();
});
#endregion

// Start
RunTarget(target);