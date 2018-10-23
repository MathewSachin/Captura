#l "constants.cake"
#l "backup.cake"
using static System.Text.RegularExpressions.Regex;

var prerelease = false;
readonly var buildNo = EnvironmentVariable("APPVEYOR_BUILD_NUMBER");

// version parameter is already used by cake.exe
var version = Argument<string>("appversion", null);

readonly var tag = Argument<string>("tag", null);

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
    var assemblyInfoFile = File("Properties/AssemblyInfo.cs");
    var uiAssemblyInfo = sourceFolder + Directory("Captura") + assemblyInfoFile;
    var consoleAssemblyInfo = sourceFolder + Directory("Captura.Console") + assemblyInfoFile;

    void DoUpdate()
    {
        // Update AssemblyInfo files
        CreateBackup(uiAssemblyInfo, tempFolder + File("AssemblyInfo.cs"));
        CreateBackup(consoleAssemblyInfo, tempFolder + File("console.cs"));

        UpdateVersion(uiAssemblyInfo);
        UpdateVersion(consoleAssemblyInfo);
    }

    if (string.IsNullOrWhiteSpace(version))
    {
        // Read from AssemblyInfo
        version = ParseAssemblyInfo(uiAssemblyInfo).AssemblyVersion;

        // Dev build
        if (version == "0.0.0" && !string.IsNullOrWhiteSpace(buildNo))
        {
            version = $"0.0.{buildNo}";

            DoUpdate();
        }
    }
    else DoUpdate();
}

void HandleTag()
{
    // Not a Tag Build
    if (string.IsNullOrWhiteSpace(tag))
        return;

    const string StableVersionRegex = @"^v\d+\.\d+\.\d+$";
    const string PrereleaseVersionRegex = @"^v\d+\.\d+\.\d+-[^\s]+$";

    // Stable Release
    if (IsMatch(tag, StableVersionRegex))
    {
        version = tag.Substring(1);
    }
    // Prerelease
    else if (IsMatch(tag, PrereleaseVersionRegex))
    {
        prerelease = true;

        version = tag.Split('-')[0].Substring(1);

        Information("Update version");
    }
    else throw new ArgumentException("Invalid Tag Format", "Tag");
}