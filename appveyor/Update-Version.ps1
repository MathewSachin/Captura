# Path to AssemblyInfo.cs file
$uiInfo = "src\Captura\Properties\AssemblyInfo.cs"
$consoleInfo = "src\Captura.Console\Properties\AssemblyInfo.cs"

# Default Version
$env:AppVersion = "1.0.0"

function Update-Version ($infoPath, $version) {
    $content = Get-Content $infoPath
    $replaced = $content -replace 'AssemblyVersion\(\".+?\"\)', "AssemblyVersion(`"$version`")"
    Set-Content $infoPath $replaced
}

# For Tag build
if ($env:appveyor_repo_tag -eq 'true')
{
    # Check tag name format
    if (-not ($env:APPVEYOR_REPO_TAG_NAME -match '^v\d+\.\d+\.\d+$'))
    {
        throw 'Invalid Tag Format'
    }

    # Extract AppVersion from Tag name
    $env:AppVersion = ($env:APPVEYOR_REPO_TAG_NAME).Substring(1)

    # Update AssemblyInfo.cs with Version from tag
    Update-Version($uiInfo, $env:AppVersion);
    Update-Version($consoleInfo, $env:AppVersion);
}
else
{
    # Retrieve Version from AssemblyInfo.cs
    $content = Get-Content $uiInfo
    $match = [regex]::Match($content, 'AssemblyVersion\(\"(.+?)\"\)')

    if ($match.Success)
    {
        $env:AppVersion = $match.Groups[1].Value
    }
}

# Update Appveyor Build Version
Update-AppveyorBuild -Version "$env:AppVersion.$env:APPVEYOR_BUILD_NUMBER"