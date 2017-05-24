# Path to AssemblyInfo.cs file
$assemblyInfo = "src\Captura\Properties\AssemblyInfo.cs"

# Default Version
$env:AppVersion = "1.0.0"

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
    $content = Get-Content $assemblyInfo
    $replaced = $content -replace 'AssemblyVersion\(\".+?\"\)', "AssemblyVersion(`"$env:AppVersion`")"
    Set-Content $assemblyInfo $replaced
}
else
{
    # Retrieve Version from AssemblyInfo.cs
    $content = Get-Content $assemblyInfo
    $match = [regex]::Match($content, 'AssemblyVersion\(\"(.+?)\"\)')

    if ($match.Success)
    {
        $env:AppVersion = $match.Groups[1].Value
    }
}

# Update Appveyor Build Version
Update-AppveyorBuild -Version "$env:AppVersion.$env:APPVEYOR_BUILD_NUMBER"