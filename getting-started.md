---
layout: page
title: Getting Started
permalink: Getting-Started/
---

## Build Notes
- [Visual Studio 2017](https://visualstudio.com) or greater is required.
- .Net Framework v4.6.1 is required.
- Run `init.bat` after cloning the repository (clones submodules).

## Optional Native Libraries
Download and place in build output directory.
These may need licensing for commercial use.

- [BASS Audio library](http://www.un4seen.com/download.php?bass24) for Audio Recording - *bass.dll* (x86).
- [BASSmix](http://www.un4seen.com/download.php?bassmix24) for Audio Loopback - *bassmix.dll* (x86).
- [FFMpeg](https://ffmpeg.zeranoe.com/builds/) for Audio/Video Encoding - *ffmpeg.exe* (Static build recommended) (x86 or x64 as per platform).

>Only _Screna.dll_ is required for Captura to run. All the other libraries add extra features if present.