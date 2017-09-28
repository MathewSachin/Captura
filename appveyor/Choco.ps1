# Prepare Chocolatey package only on Release Tag builds
if ($env:configuration -eq 'Release' -and $env:appveyor_repo_tag -eq 'true')
{
    # sha256 Checksum
    $checksum = (Get-FileHash "Output/Captura-Portable.zip").Hash

    $installScript = "choco/tools/chocolateyinstall.ps1"

    $newContent = @(('$tag = "{0}";' -f $env:APPVEYOR_REPO_TAG_NAME), ('$checksum = "{0}";' -f $checksum), (Get-Content $installScript))

    # Update chocolatey install script
    Set-Content $installScript $newContent

    # Pack Chocolatey Package with Tag version
    choco pack choco/captura.nuspec --version $env:TagVersion
}