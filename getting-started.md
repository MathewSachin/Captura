---
layout: page
title: Getting Started
permalink: Getting-Started/
---

## Want to build yourself?
- [Visual Studio 2017](https://visualstudio.com) or greater is required.
- .Net Framework v4.6.1 is required.
- **Clone submodules:** Open a terminal in the project folder and type `git submodule update --init`.
- Restore NuGet packages.

### Dependencies as Submodules
- Screna

### Dependencies installed from NuGet.
- SharpAvi
- MouseKeyHook
- ManagedBass
- NAudio
- HardCodet.Wpf.TaskbarNotification
- WebEye.Wpf.WebCameraControl

### Native dependencies.
Download [BASS Audio library](http://www.un4seen.com/download.php?bass24) and [BASSmix](http://www.un4seen.com/download.php?bassmix24).

`bass.dll` and `bassmix.dll` (x86 versions, since Capture runs in `Prefer32Bit` mode) need to be placed in the same directory as `Captura.exe`.

Download `ffmpeg.exe` and place in any of the following:
- A folder in `PATH` environment variable.
- Build output folder.
- FFMpeg folder selected in the App.

## Want prebuilt libraries?
- Releases are available on [GitHub Releases]({{ site.links.github }}/releases) or from [Downloads]({{ site.baseurl }}/Downloads) page.
- Chocolatey: `choco install captura -y`
- See [here]({{ site.baseurl }}/CI) for how to get **Dev Builds**.
- FFMpeg is not provided with the releases. You can download it manually if you want.

**These libraries are optional:** SharpAvi, FFMpeg, MouseKeyHook, ManagedBass and NAudio (Naudio is used as a fallback when ManagedBass cannot be used).