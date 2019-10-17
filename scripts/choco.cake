#l "constants.cake"
#l "backup.cake"
#l "version.cake"

readonly var chocoVersion = tag?.Substring(1) ?? "";

readonly var ChocoPkgPath = tempFolder + File($"captura.{chocoVersion}.nupkg");

void PackChoco()
{
    var checksum = CalculateFileHash(PortablePath).ToHex();

    var chocoInstallScript = chocoFolder + File("tools/chocolateyinstall.ps1");

    var originalContent = FileRead(chocoInstallScript);

    var newContent = $"$tag = '{tag}'; $checksum = '{checksum}'; {originalContent}";

    CreateBackup(chocoInstallScript, tempFolder + File("cinst.ps1"));

    FileWrite(chocoInstallScript, newContent);

    ChocolateyPack(chocoFolder + File("captura.nuspec"), new ChocolateyPackSettings
    {
        Version = chocoVersion,
        ArgumentCustomization = Args => Args.Append($"--outputdirectory {tempFolder}")
    });
}