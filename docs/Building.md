# Building

## Requirements

- [Visual Studio 2017](https://visualstudio.com) or greater is recommended. Can also be built using Visual Studio 2015 using [Microsoft.Net.Compilers](https://www.nuget.org/packages/Microsoft.Net.Compilers) nuget package to support compilation of C# 7.
- .Net Framework v4.6.1 is required.

### Api Keys

Api Keys are loaded from user environment variables during development and embedded in Release builds if using Cake.

Service | Environment Variable
--------|----------------------
Imgur   | `imgur_client_id`

### FFMpeg

FFMpeg can be downloaded from within the app.

## Building using Cake

We use Cake (C# Make) to automate the build process.

To do a Release build, open powershell in project folder and type: `./build.ps1`.

The build is then available in the `Output` folder.

### Options Supported by build script

Option           | Description
-----------------|-------------
`-version`       | Build version. `AssemblyInfo.cs` files are updated with this value during build.
`-configuration` | Configuration: *Release* or *Debug*
`-target`        | The Task to run in the build script. See `build.cake`

e.g. `./build.ps1 -target "Pack-Setup" -version 5.6.0`

## Building Manually

Some extra things that need to be done when building manually.

### NuGet

Restore NuGet packages.

### Audio

Download [BASS Audio library](http://www.un4seen.com/download.php?bass24) and [BASSmix](http://www.un4seen.com/download.php?bassmix24).

`bass.dll` and `bassmix.dll` (x86 versions, since Capture runs in `Prefer32Bit` mode) need to be placed in the same directory as `Captura.exe`.