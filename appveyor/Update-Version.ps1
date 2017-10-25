# For Tag build
if ($env:appveyor_repo_tag -eq 'true')
{
    $env:TagVersion = $env:APPVEYOR_REPO_TAG_NAME.Substring(1)

    # Check tag name format
    if ($env:APPVEYOR_REPO_TAG_NAME -match '^v\d+\.\d+\.\d+$')
    {
        $env:prerelease = $false

        # Extract AppVersion from Tag name
        $env:AppVersion = $env:TagVersion
    }
    elseif ($env:APPVEYOR_REPO_TAG_NAME -match '^v\d+\.\d+\.\d+-[^\s]+$')
    {
        $env:prerelease = $true

        $env:AppVersion = $env:TagVersion.Split('-')[0]
    }
    else
    {
        throw 'Invalid Tag Format'
    }

    # Update Appveyor Build Version
    Update-AppveyorBuild -Version "$env:AppVersion.$env:APPVEYOR_BUILD_NUMBER"
}