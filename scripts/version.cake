#l "constants.cake"
#l "backup.cake"
using static System.Text.RegularExpressions.Regex;

// version parameter is already used by cake.exe
var tag = Argument<string>("build_version", "v0.0.0");
var version = tag;

void UpdateVersion(string AssemblyInfoPath)
{
    var content = FileRead(AssemblyInfoPath);

    var start = content.IndexOf("AssemblyVersion");
    var end = content.IndexOf(")", start);

    var replace = content.Replace(content.Substring(start, end - start + 1), $"AssemblyVersion(\"{version}\")");

    FileWrite(AssemblyInfoPath, replace);
}

void HandleVersion()
{
    const string StableVersionRegex = @"^v\d+\.\d+\.\d+$";
    const string PrereleaseVersionRegex = @"^v\d+\.\d+\.\d+-[^\s]+$";

    // Stable Release or CI build
    if (IsMatch(version, StableVersionRegex))
    {
        version = version.Substring(1);
    }
    // Prerelease
    else if (IsMatch(version, PrereleaseVersionRegex))
    {
        version = version.Split('-')[0].Substring(1);
    }
    else throw new ArgumentException("Invalid Version Format", "build_version");

    var assemblyInfoFile = File("Properties/AssemblyInfo.cs");
    var uiAssemblyInfo = sourceFolder + Directory("Captura") + assemblyInfoFile;
    var consoleAssemblyInfo = sourceFolder + Directory("Captura.Console") + assemblyInfoFile;

    // Update AssemblyInfo files
    CreateBackup(uiAssemblyInfo, tempFolder + File("AssemblyInfo.cs"));
    CreateBackup(consoleAssemblyInfo, tempFolder + File("console.cs"));

    UpdateVersion(uiAssemblyInfo);
    UpdateVersion(consoleAssemblyInfo);
}