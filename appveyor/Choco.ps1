# Prepare Chocolatey package only on Release Tag builds
if ($env:configuration -eq 'Release' -and $env:appveyor_repo_tag -eq 'true')
{
    # sha256 Checksum
    $checksum = (Get-FileHash "Output/Captura-Release.zip").Hash

    $installScript = "choco/tools/chocolateyinstall.ps1"

    $scriptContent = "`n`n" + (Get-Content $installScript)

    # Update chocolatey install script
    Set-Content $installScript (('$tag = "{0}"; $checksum = "{1}";{2}') -f $env:APPVEYOR_REPO_TAG_NAME, $checksum, $scriptContent)

    # Pack Chocolatey Package with Tag version
    choco pack choco/captura.nuspec --version $env:AppVersion

    # Upload as Artifact. Can be later published using Deploy button.
    Push-AppVeyorArtifact "captura.$env:AppVersion.nupkg" -DeploymentName Chocolatey
}