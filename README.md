# Captura
[![Build Status](https://img.shields.io/appveyor/ci/MathewSachin/Captura.svg?style=flat-square)](https://ci.appveyor.com/project/MathewSachin/Captura)
[![MIT License](https://img.shields.io/github/license/MathewSachin/Captura.svg?style=flat-square)](LICENSE.md)
[![Chat](https://img.shields.io/gitter/room/MathewSachin/Screna.svg?style=flat-square)](https://gitter.im/MathewSachin/Screna)  
&copy; [Copyright 2017](LICENSE.md) Mathew Sachin

Capture Screen, Audio, Cursor, Mouse Clicks and Keystrokes using [Screna](https://github.com/MathewSachin/Screna).

![ScreenShot](http://mathewsachin.github.io/Captura/ScreenShots/img/expanded.png)

[See more ScreenShots](http://mathewsachin.github.io/Captura/ScreenShots/)

# Features
- Take ScreenShots
- Capture ScreenCasts (Avi/Gif/Mp4)
- Capture with/without Mouse Cursor
- Capture Specific Regions or Windows
- Capture Mouse Clicks or Keystrokes
- Record Audio from Microphone **AND/OR** Speaker Output (Wasapi Loopback)
- Modular (Only _Screna.dll_ is a required dependency).

# Build Notes
- [Visual Studio 2017](https://visualstudio.com) or greater is required.
- Run `init.bat` after cloning the repository (clones submodules).

## Optional Native Libraries
Download and place in build output directory (x86 or x64 as required).
These may need licensing for commercial use.
x64 libraries are available in [Releases](https://github.com/MathewSachin/Captura/releases).

- [BASS Audio library](http://www.un4seen.com/download.php?bass24) for Audio Recording - *bass.dll*.
- [BASSmix](http://www.un4seen.com/download.php?bassmix24) for Audio Mixing - *bassmix.dll*.
- [FFMpeg](https://ffmpeg.zeranoe.com/builds/) for Audio/Video Encoding - *ffmpeg.exe* (Static build recommended).