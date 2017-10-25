var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = Argument<string>("appversion", null);

const string slnPath = "src/Captura.sln";
const string apiKeysPath = "src/Captura.Core/ApiKeys.cs";
const string imgurEnv = "imgur_client_id";
const string uiAssemblyInfo = "src/Captura/Properties/AssemblyInfo.cs";
const string consoleAssemblyInfo = "src/Captura.Console/Properties/AssemblyInfo.cs";

bool apiKeyEmbed, assemblyInfoUpdate;

void UpdateVersion(string AssemblyInfoPath)
{
    var content = System.IO.File.ReadAllText(AssemblyInfoPath);

    var start = content.IndexOf("AssemblyVersion");
    var end = content.IndexOf(")", start);

    var replace = content.Replace(content.Substring(start, end - start + 1), $"AssemblyVersion(\"{version}\")");

    System.IO.File.WriteAllText(AssemblyInfoPath, replace);
}

Setup(context =>
{
    if (string.IsNullOrWhiteSpace(version))
    {
        // Read from AssemblyInfo
        version = ParseAssemblyInfo(uiAssemblyInfo).AssemblyVersion;
    }
    else
    {
        assemblyInfoUpdate = true;

        CopyFileToDirectory(uiAssemblyInfo, "temp");
        CopyFile(consoleAssemblyInfo, "temp/console.cs");

        UpdateVersion(uiAssemblyInfo);
        UpdateVersion(consoleAssemblyInfo);
    }

    // Embed Api Keys in Release builds
    if (configuration == "Release" && HasEnvironmentVariable(imgurEnv))
    {
        apiKeyEmbed = true;

        EnsureDirectoryExists("temp");

        Information("Embedding Api Keys from Environment Variables ...");

        CopyFileToDirectory(apiKeysPath, "temp");

        var apiKeysOriginalContent = System.IO.File.ReadAllText(apiKeysPath);

        var newContent = apiKeysOriginalContent.Replace($"Get(\"{imgurEnv}\")", $"\"{EnvironmentVariable(imgurEnv)}\"");

        System.IO.File.WriteAllText(apiKeysPath, newContent);
    }
});

Teardown(context =>
{
    if (apiKeyEmbed)
    {
        DeleteFile(apiKeysPath);
        MoveFile("temp/ApiKeys.cs", apiKeysPath);
    }

    if (assemblyInfoUpdate)
    {
        DeleteFile(uiAssemblyInfo);
        DeleteFile(consoleAssemblyInfo);

        MoveFile("temp/AssemblyInfo.cs", uiAssemblyInfo);
        MoveFile("temp/console.cs", consoleAssemblyInfo);
    }
});

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
    Zip("Output", "temp/Captura-Portable.zip");
});

Task("Pack-Setup")
    .WithCriteria(configuration == "Release")
    .IsDependentOn("Populate-Output")
    .Does(() =>
{
    InnoSetup("Inno.iss", new InnoSetupSettings
    {
        QuietMode = InnoSetupQuietMode.Quiet,
        ArgumentCustomization = Args => Args.Append($"/DMyAppVersion={version}")
    });
});

Task("Pack-Choco")
    // Run only on AppVeyor Release Tag builds
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor && configuration == "Release" && HasEnvironmentVariable("APPVEYOR_REPO_TAG_NAME"))
    .IsDependentOn("Pack-Portable")
    .Does(() =>
{
    var checksum = CalculateFileHash("temp/Captura-Portable.zip").ToHex();

    var chocoInstallScript = "choco/tools/chocolateyinstall.ps1";

    var originalContent = System.IO.File.ReadAllText(chocoInstallScript);

    var tag = EnvironmentVariable("APPVEYOR_REPO_TAG_NAME");
    var newContent = $"$tag = '{tag}'; $checksum = '{checksum}'; {originalContent}";

    CopyFileToDirectory(chocoInstallScript, "temp");

    System.IO.File.WriteAllText(chocoInstallScript, newContent);

    var chocoVersion = EnvironmentVariable("TagVersion");

    ChocolateyPack("choco/captura.nuspec", new ChocolateyPackSettings
    {
        Version = chocoVersion,
        ArgumentCustomization = Args => Args.Append("--outputdirectory temp")
    });

    DeleteFile(chocoInstallScript);
    MoveFile("temp/chocolateyinstall.ps1", chocoInstallScript);
});

Task("Default").IsDependentOn("Populate-Output");

Task("CI")
    .IsDependentOn("Pack-Portable")
    .IsDependentOn("Pack-Setup")
    .IsDependentOn("Pack-Choco");

RunTarget(target);