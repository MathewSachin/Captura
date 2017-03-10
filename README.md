# Captura
[![Build status](https://ci.appveyor.com/api/projects/status/cgobcowf79uc5dx0/branch/master??svg=true)](https://ci.appveyor.com/project/MathewSachin/captura)
[![MIT License](https://img.shields.io/github/license/MathewSachin/Captura.svg)](LICENSE.md)
[![Gitter](https://badges.gitter.im/MathewSachin/Screna.svg)](https://gitter.im/MathewSachin/Screna)  
&copy; [Copyright 2017](LICENSE.md) Mathew Sachin

Capture Screen, Audio, Cursor, Mouse Clicks and Keystrokes using [Screna](https://github.com/MathewSachin/Screna).

![ScreenShot](https://raw.githubusercontent.com/wiki/MathewSachin/Captura/img/expanded.png)

# Features
- Take ScreenShots
- Capture ScreenCasts (Avi/Gif)
- Capture with/without Mouse Cursor
- Capture Specific Regions or Windows
- Capture Mouse Clicks or Keystrokes
- Record Audio from Microphone **AND/OR** Speaker Output (Wasapi Loopback)
- Modular (Only _Screna.dll_ is a required dependency).

# Getting Started
1. [Visual Studio 2017](https://visualstudio.com) or greater is required.
2. Clone the repository: `git clone https://github.com/MathewSachin/Captura.git`.
3. Fetch Submodules: `git submodule init` and `git submodule update`.

## Optional Native Libraries
Download and place in build output directory (x86 or x64 as required).
These may need licensing for commercial use.
x64 libraries are available in [Releases](https://github.com/MathewSachin/Captura/releases).

### Audio Recording/Loopback and Mixing
- [BASS Audio library](http://www.un4seen.com/download.php?bass24) - *bass.dll*.
- [BASSmix](http://www.un4seen.com/download.php?bassmix24) - *bassmix.dll*.

### Video and/or Audio Encoding
- [FFMpeg](https://ffmpeg.zeranoe.com/builds/) - *ffmpeg.exe* (Static build recommended).

# Acknowledgements
- [SharpAvi](https://github.com/bassill/sharpavi) for helping improve the idea.
- [Material Design Icons](https://materialdesignicons.com) for the Icons.
- [Modern UI](https://github.com/firstfloorsoftware/mui) for the styles.