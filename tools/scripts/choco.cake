#l "constants.cake"
#l "backup.cake"

void PackChoco(string Tag, string Version)
{
    var checksum = CalculateFileHash(PortablePath).ToHex();

    var chocoInstallScript = chocoFolder + File("tools/chocolateyinstall.ps1");

    var originalContent = FileRead(chocoInstallScript);

    var newContent = $"$tag = '{Tag}'; $checksum = '{checksum}'; {originalContent}";

    CreateBackup(chocoInstallScript, tempFolder + File("cinst.ps1"));

    FileWrite(chocoInstallScript, newContent);

    ChocolateyPack(chocoFolder + File("captura.nuspec"), new ChocolateyPackSettings
    {
        Version = Version,
        ArgumentCustomization = Args => Args.Append($"--outputdirectory {tempFolder}")
    });
}

void PushChoco(string PackagePath)
{
    ChocolateyPush(PackagePath, new ChocolateyPushSettings
    {
        ApiKey = EnvironmentVariable("choco_key")
    });
}