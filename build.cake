var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var slnPath = "src/Captura.sln";

Task("Clean").Does(() =>
{
    DotNetBuild(slnPath, settings =>
    {
        settings.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("Clean");
    });
});

Task("Nuget-Restore").Does(() =>
{
    NuGetRestore(slnPath);
});

// Restores native dlls
void NativeRestore()
{
    EnsureDirectoryExists("temp");

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
        CopyFileToDirectory(bass, output);
        CopyFileToDirectory(bassmix, output);
    }
}

Task("Build")
    .IsDependentOn("NuGet-Restore")
    .Does(() =>
{
    DotNetBuild(slnPath, settings =>
    {
        settings.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("Rebuild");
    });

    NativeRestore();
});

Task("Clean-Output").Does(() =>
{
    CleanDirectory("Output");
});

Task("Populate-Output")
    .IsDependentOn("Clean-Output")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Copy License files
    CopyDirectory("licenses", "Output/licenses");

    var binFolder = $"src/Captura.Console/bin/{configuration}/";

    // Copy Assemblies
    CopyFiles(binFolder + "*.dll", "Output");
    
    // Copy Resource Assemblies
    CopyFiles(binFolder + "*/*.dll", "Output", true);

    // Copy executables and config files
    CopyFiles(binFolder + "*.exe*", "Output");

    // For Debug builds
    if (configuration == "Debug")
    {
        // Copy symbol files
        CopyFiles(binFolder + "*.pdb", "Output");

        // Copy Xml Documentation
        CopyFiles(binFolder + "*.xml", "Output");
    }
});

Task("Pack-Portable")
    .IsDependentOn("Populate-Output")
    .Does(() =>
{
    Zip("Output", "Captura-Portable.zip");
});

Task("Pack-Setup")
    .WithCriteria(() => configuration == "Release")
    .IsDependentOn("Populate-Output")
    .Does(() =>
{
    InnoSetup("Inno.iss");
});

Task("Default").IsDependentOn("Build");

Task("CI")
    .IsDependentOn("Pack-Portable")
    .IsDependentOn("Pack-Setup");

RunTarget(target);