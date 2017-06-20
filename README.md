# Captura
[![Master Build Status](https://img.shields.io/appveyor/ci/MathewSachin/Captura/master.svg?style=flat-square)](https://ci.appveyor.com/project/MathewSachin/Captura)
[![MIT License](https://img.shields.io/github/license/MathewSachin/Captura.svg?style=flat-square)](LICENSE.md)
[![Chat](https://img.shields.io/gitter/room/MathewSachin/Screna.svg?style=flat-square)](https://gitter.im/MathewSachin/Screna)  
&copy; [Copyright 2017](LICENSE.md) Mathew Sachin

http://mathewsachin.github.io/Captura/

Capture Screen, WebCam, Audio, Cursor, Mouse Clicks and Keystrokes using [Screna](https://github.com/MathewSachin/Screna).

![ScreenShot](http://mathewsachin.github.io/Captura/img/ScreenShots/Tabs/Main.png)

[See more ScreenShots](http://mathewsachin.github.io/Captura/ScreenShots/)

# Available on Chocolatey
```
choco install captura -y
```

# Features
- Take ScreenShots
- Capture ScreenCasts (Avi/Gif/Mp4)
- Capture with/without Mouse Cursor
- Capture Specific Regions, Screens or Windows
- Capture Mouse Clicks or Keystrokes
- Mix Audio recorded from Microphone and Speaker Output
- Capture from WebCam.
- Can be used from [Command-line](http://mathewsachin.github.io/Captura/Manual/cmdline.html).

# Build Notes
- [Visual Studio 2017](https://visualstudio.com) or greater is recommended. Can also be build using Visual Studio 2015 using [Microsoft.Net.Compilers](https://www.nuget.org/packages/Microsoft.Net.Compilers) nuget package to support compilation of C# 7.
- .Net Framework v4.6.1 is required.
- **Clone submodules:** Open a terminal in the project folder and type `git submodule update --init`.

## Optional Native Libraries
Download and place in build output directory.
These may need licensing for commercial use.

- [BASS Audio library](http://www.un4seen.com/download.php?bass24) for Audio Recording - *bass.dll* (x86).
- [BASSmix](http://www.un4seen.com/download.php?bassmix24) for Audio Mixing - *bassmix.dll* (x86).
- [FFMpeg](https://ffmpeg.zeranoe.com/builds/) for Audio/Video Encoding - *ffmpeg.exe* (Static build recommended) (x86 or x64 as per platform).