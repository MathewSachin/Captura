#tool nuget:?package=xunit.runner.console&version=2.4.1
#l "scripts/backup.cake"
#l "scripts/constants.cake"
#l "scripts/choco.cake"
#l "scripts/apikeys.cake"
#l "scripts/version.cake"
using System.Collections.Generic;

#region Fields
readonly var target = Argument("target", "Default");
readonly var configuration = Argument("configuration", Release);
#endregion

#region Functions
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

void PackPortable()
{
    // Portable build directories
    var settingsDir = distFolder + Directory("Settings");
    var codecsDir = distFolder + Directory("Codecs");

    CreateDirectory(settingsDir);
    CreateDirectory(codecsDir);

    Zip(distFolder, PortablePath);

    var dirDeleteSettings = new DeleteDirectorySettings {
        Recursive = true,
        Force = true
    };

    DeleteDirectory(settingsDir, dirDeleteSettings);
    DeleteDirectory(codecsDir, dirDeleteSettings);
}
#endregion

#region Setup / Teardown
Setup(context =>
{
    EnsureDirectoryExists(tempFolder);
    EnsureDirectoryExists(distFolder);

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
var buildTask = Task("Build")
    .Does(() =>
{
    MSBuild(slnPath, settings =>
    {
        settings.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("Rebuild")
            .WithRestore()
            .UseToolVersion(MSBuildToolVersion.VS2019);
    });
});

var cleanOutputTask = Task("Clean-Output").Does(() => CleanDirectory(distFolder));

var populateOutputTask = Task("Populate-Output")
    .IsDependentOn(cleanOutputTask)
    .IsDependentOn(buildTask)
    .Does(() => PopulateOutput());

var packPortableTask = Task("Pack-Portable")
    .IsDependentOn(populateOutputTask)
    .Does(() => PackPortable());

var packSetupTask = Task("Pack-Setup")
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

var packChocoTask = Task("Pack-Choco")
    .IsDependentOn(packPortableTask)
    .Does(() => PackChoco());

var testTask = Task("Test")
    .IsDependentOn(buildTask)
    .Does(() => XUnit2(sourceFolder + File($"Tests/bin/{configuration}/**/Captura.Tests.dll")));

var defaultTask = Task("Default")
    .IsDependentOn(packPortableTask)
    .IsDependentOn(packSetupTask)
    .IsDependentOn(packChocoTask);
#endregion

Task("CI")
    .IsDependentOn(testTask)
    .IsDependentOn(packPortableTask)
    .IsDependentOn(packSetupTask)
    .IsDependentOn(packChocoTask)
    .Does(() => { });

// Start
RunTarget(target);