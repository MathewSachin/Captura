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
Task("Native-Restore").Does(() =>
{
    EnsureDirectoryExists("temp");

    var bass = "temp/bass/bass.dll";
    var bassmix = "temp/bassmix/bassmix.dll";

    if (!FileExists(bass))
    {
        DownloadFile("http://www.un4seen.com/files/bass24.zip", "temp/bass.zip");

        Unzip("temp/bass.zip", "temp/bass");
    }

    if (!FileExists(bassmix))
    {
        DownloadFile("http://www.un4seen.com/files/bassmix24.zip", "temp/bassmix.zip");

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
});

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

    RunTarget("Native-Restore");
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