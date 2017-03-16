---
layout: page
title: Getting Started
---

## Build Notes
- [Visual Studio 2017](https://visualstudio.com) or greater is required.
- Run `init.bat` after cloning the repository (clones submodules).

## Optional Native Libraries
Download and place in build output directory (x86 or x64 as required).
These may need licensing for commercial use.
x64 libraries are available in [Releases](https://github.com/MathewSachin/Captura/releases).

- [BASS Audio library](http://www.un4seen.com/download.php?bass24) for Audio Recording - *bass.dll*.
- [BASSmix](http://www.un4seen.com/download.php?bassmix24) for Audio Loopback - *bassmix.dll*.
- [FFMpeg](https://ffmpeg.zeranoe.com/builds/) for Audio/Video Encoding - *ffmpeg.exe* (Static build recommended).

>Only _Screna.dll_ is required for Captura to run. All the other libraries add extra features if present.