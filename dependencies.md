---
layout: page
title: Dependencies
permalink: Dependencies/
---

## Required Dependencies

### [Screna](https://github.com/MathewSachin/Screna)
Screna does the real work of capturing Screen.

[![NuGet](https://img.shields.io/nuget/dt/Screna.svg?style=flat-square&label=nuget)](https://nuget.org/packages/Screna)
[![Stars](https://img.shields.io/github/stars/MathewSachin/Screna.svg?style=flat-square)](https://github.com/MathewSachin/Screna/stargazers)
[![Forks](https://img.shields.io/github/forks/MathewSachin/Screna.svg?style=flat-square)](https://github.com/MathewSachin/Screna/fork)
[![Issues](https://img.shields.io/github/issues/MathewSachin/Screna.svg?style=flat-square)](https://github.com/MathewSachin/Screna/issues)

[![Gratipay](https://img.shields.io/gratipay/team/Screna.svg?style=flat-square)](https://gratipay.com/Screna/)
[![Build Status](https://img.shields.io/appveyor/ci/MathewSachin/Screna.svg?style=flat-square)](https://ci.appveyor.com/project/MathewSachin/Screna)
[![Chat](https://img.shields.io/gitter/room/MathewSachin/Screna.svg?style=flat-square)](https://gitter.im/MathewSachin/Screna)

-----------------------------

## Optional Dependencies
These dependencies are NOT required, but add extra features if present.

### [FFMpeg](https://ffmpeg.zeranoe.com/builds/)
Audio/Video encoding support.

Download `ffmpeg.exe` and place in any folder in `PATH` environment variable or in the same directory as `Captura.exe`.

### [MouseKeyHook](https://github.com/gmamaladze/globalmousekeyhook)
Capture Mouse Clicks and Keystrokes as Overlays.

[![NuGet](https://img.shields.io/nuget/dt/MouseKeyHook.svg?style=flat-square&label=nuget)](https://nuget.org/packages/MouseKeyHook)
[![Stars](https://img.shields.io/github/stars/gmamaladze/globalmousekeyhook.svg?style=flat-square)](https://github.com/gmamaladze/globalmousekeyhook/stargazers)
[![Forks](https://img.shields.io/github/forks/gmamaladze/globalmousekeyhook.svg?style=flat-square)](https://github.com/gmamaladze/globalmousekeyhook/fork)
[![Issues](https://img.shields.io/github/issues/gmamaladze/globalmousekeyhook.svg?style=flat-square)](https://github.com/gmamaladze/globalmousekeyhook/issues)

### [SharpAvi](https://github.com/bassill/sharpavi/)
Avi encodig support.

[![NuGet](https://img.shields.io/nuget/dt/SharpAvi.svg?style=flat-square&label=nuget)](https://nuget.org/packages/SharpAvi)
[![Stars](https://img.shields.io/github/stars/bassill/sharpavi.svg?style=flat-square)](https://github.com/bassill/sharpavi/stargazers)
[![Forks](https://img.shields.io/github/forks/bassill/sharpavi.svg?style=flat-square)](https://github.com/bassill/sharpavi/fork)
[![Issues](https://img.shields.io/github/issues/bassill/sharpavi.svg?style=flat-square)](https://github.com/bassill/sharpavi/issues)

### [ManagedBass](https://github.com/ManagedBass/ManagedBass)
Microphone Recording and Speaker Output (Wasapi Loopback) capture with mixing support.

Download [BASS Audio library](http://www.un4seen.com/download.php?bass24) and [BASSmix](http://www.un4seen.com/download.php?bassmix24).

`bass.dll` and `bassmix.dll` (x86 versions, since Capture runs in `Prefer32Bit` mode) need to be placed in the same directory as `Captura.exe`.

[![NuGet](https://img.shields.io/nuget/dt/ManagedBass.svg?style=flat-square&label=nuget)](https://nuget.org/packages/ManagedBass)
[![Stars](https://img.shields.io/github/stars/ManagedBass/ManagedBass.svg?style=flat-square)](https://github.com/ManagedBass/ManagedBass/stargazers)
[![Forks](https://img.shields.io/github/forks/ManagedBass/ManagedBass.svg?style=flat-square)](https://github.com/ManagedBass/ManagedBass/fork)
[![Issues](https://img.shields.io/github/issues/ManagedBass/ManagedBass.svg?style=flat-square)](https://github.com/ManagedBass/ManagedBass/issues)

[![Gratipay](https://img.shields.io/gratipay/team/ManagedBass.svg?style=flat-square)](https://gratipay.com/ManagedBass/)
[![Build Status](https://img.shields.io/appveyor/ci/MathewSachin/ManagedBass.svg?style=flat-square)](https://ci.appveyor.com/project/MathewSachin/managedbass)
[![Chat](https://img.shields.io/gitter/room/ManagedBass/ManagedBass.svg?style=flat-square)](https://gitter.im/ManagedBass/ManagedBass)

### [NAudio](https://github.com/NAudio/NAudio)
Microphone Recording support when ManagedBass is not present.

[![NuGet](https://img.shields.io/nuget/dt/NAudio.svg?style=flat-square&label=nuget)](https://nuget.org/packages/NAudio)
[![Stars](https://img.shields.io/github/stars/naudio/naudio.svg?style=flat-square)](https://github.com/naudio/naudio/stargazers)
[![Forks](https://img.shields.io/github/forks/naudio/naudio.svg?style=flat-square)](https://github.com/naudio/naudio/fork)
[![Issues](https://img.shields.io/github/issues/naudio/naudio.svg?style=flat-square)](https://github.com/naudio/naudio/issues)