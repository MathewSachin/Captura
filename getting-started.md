---
layout: page
title: Getting Started
permalink: Getting-Started/
reading_time: true
---

## Want prebuilt libraries?
- Releases are available on [GitHub Releases]({{ site.links.github }}/releases) or from [Downloads]({{ site.baseurl }}/Downloads) page.
- Chocolatey: `choco install captura -y`
- See [here]({{ site.baseurl }}/CI) for how to get **Dev Builds**.
- FFmpeg is not provided with the releases. You can download it manually if you want.
- Captura can work even if any or all of these libraries are missing: SharpAvi, FFmpeg, MouseKeyHook, ManagedBass.

## Want to build yourself?
- [Visual Studio 2017](https://visualstudio.com) or greater is recommended. Can also be build using Visual Studio 2015 using [Microsoft.Net.Compilers](https://www.nuget.org/packages/Microsoft.Net.Compilers) nuget package to support compilation of C# 7.
- .Net Framework v4.6.1 is required.
- **Clone submodules:** Open a terminal in the project folder and type `git submodule update --init`.
- Restore NuGet packages.

### Dependencies as Submodules
- [Screna](https://github.com/MathewSachin/Screna)

### Dependencies installed from NuGet.
- SharpAvi
- MouseKeyHook
- ManagedBass
- HardCodet.Wpf.TaskbarNotification
- WebEye.Wpf.WebCameraControl
- WPFCustomMessageBox
- CommandLineParser

### Native dependencies.
Download [BASS Audio library](http://www.un4seen.com/download.php?bass24) and [BASSmix](http://www.un4seen.com/download.php?bassmix24).

`bass.dll` and `bassmix.dll` (x86 versions, since Capture runs in `Prefer32Bit` mode) need to be placed in the same directory as `Captura.exe`.

Download `ffmpeg.exe` and place in any of the following:
- A folder in `PATH` environment variable.
- Build output folder.
- FFmpeg folder selected in the App.
