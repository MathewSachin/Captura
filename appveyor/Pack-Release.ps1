# Create output and temp folder
md Output | Out-Null
md temp | Out-Null

# Copy licenses
Copy-Item licenses Output -Recurse

# Copy build output
if ($env:configuration -eq 'Release')
{
    Get-ChildItem -Path 'src/Captura.Console/bin/Release/*' -Include *.exe*, *.dll | Copy-Item -Destination 'Output/'
}
elseif ($env:configuration -eq 'Debug')
{
    Get-ChildItem -Path 'src/Captura.Console/bin/Debug/*' -Include *.exe*, *.dll, *.pdb, *.xml | Copy-Item -Destination 'Output/'
}

# Copy Resource Assemblies
Get-ChildItem "src/Captura.Console/bin/$env:configuration/*" -Directory | Copy-Item -Destination 'Output/' -Recurse

# Download BASS and BassMix
Invoke-WebRequest "http://www.un4seen.com/files/bass24.zip" -OutFile "temp/bass.zip"
Invoke-WebRequest "http://www.un4seen.com/files/bassmix24.zip" -OutFile "temp/bassmix.zip"

# Extract BASS and BassMix
Expand-Archive "temp/bass.zip" "temp/bass"
Expand-Archive "temp/bassmix.zip" "temp/bassmix"

# Copy BASS and BassMix
Copy-Item 'temp/bass/bass.dll', 'temp/bassmix/bassmix.dll' 'Output/'

# Pack Deploy zip
Compress-Archive 'Output\*' "Captura-Portable.zip"

if ($env:configuration -eq 'Release' -and $env:appveyor_repo_tag -eq 'true')
{
    # Install Inno Setup
    choco install innosetup -y

    # Update Version in Inno Script
    Set-Content "Inno.iss" (@('#define MyAppVersion "{0}"' -f $env:AppVersion) + (Get-Content "Inno.iss"))

    # Compile Setup
    iscc "Inno.iss"
}