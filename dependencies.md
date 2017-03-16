---
layout: page
title: Dependencies
---

<script src="scripts/nugetDownloadsBadge.js"></script>
<script>
$(function ()
{
    NuGetDownloadBadge("ManagedBass", "MBassNuGet");
    NuGetDownloadBadge("Screna", "ScrenaNuGet");
    NuGetDownloadBadge("SharpAvi", "SharpAviNuGet");
    NuGetDownloadBadge("NAudio", "NAudioNuGet");
    NuGetDownloadBadge("MouseKeyHook", "MouseKeyHookNuGet");
});
</script>

## Required Dependencies

### [Screna](https://github.com/MathewSachin/Screna)
Screna does the real work of capturing Screen.

<a href="https://nuget.org/packages/Screna" id="ScrenaNuGet"></a>
[![Stars](https://img.shields.io/github/stars/MathewSachin/Screna.svg?style=flat-square)](https://github.com/MathewSachin/Screna/stargazers)
[![Forks](https://img.shields.io/github/forks/MathewSachin/Screna.svg?style=flat-square)](https://github.com/MathewSachin/Screna/fork)
[![Issues](https://img.shields.io/github/issues/MathewSachin/Screna.svg?style=flat-square)](https://github.com/MathewSachin/Screna/issues)

[![Gratipay](https://img.shields.io/gratipay/team/Screna.svg?style=flat-square)](https://gratipay.com/Screna/)
[![Build Status](https://img.shields.io/appveyor/ci/MathewSachin/Screna.svg?style=flat-square)](https://ci.appveyor.com/project/MathewSachin/Screna)
[![Chat](https://img.shields.io/gitter/room/MathewSachin/Screna.svg?style=flat-square)](https://gitter.im/MathewSachin/Screna)

-----------------------------

## Optional Dependencies

### FFMpeg
Audio/Video encoding support.

`ffmpeg.exe` needs to be placed in the same directory as `Captura.exe`.

### [MouseKeyHook](https://github.com/gmamaladze/globalmousekeyhook)
Capture Mouse Clicks and Keystrokes as Overlays.

<a href="https://nuget.org/packages/MouseKeyHook" id="MouseKeyHookNuGet"></a>
[![Stars](https://img.shields.io/github/stars/gmamaladze/globalmousekeyhook.svg?style=flat-square)](https://github.com/gmamaladze/globalmousekeyhook/stargazers)
[![Forks](https://img.shields.io/github/forks/gmamaladze/globalmousekeyhook.svg?style=flat-square)](https://github.com/gmamaladze/globalmousekeyhook/fork)
[![Issues](https://img.shields.io/github/issues/gmamaladze/globalmousekeyhook.svg?style=flat-square)](https://github.com/gmamaladze/globalmousekeyhook/issues)

### [SharpAvi](https://github.com/bassill/sharpavi/)
Avi encodig support.

<a href="https://nuget.org/packages/SharpAvi" id="SharpAviNuGet"></a>
[![Stars](https://img.shields.io/github/stars/bassill/sharpavi.svg?style=flat-square)](https://github.com/bassill/sharpavi/stargazers)
[![Forks](https://img.shields.io/github/forks/bassill/sharpavi.svg?style=flat-square)](https://github.com/bassill/sharpavi/fork)
[![Issues](https://img.shields.io/github/issues/bassill/sharpavi.svg?style=flat-square)](https://github.com/bassill/sharpavi/issues)

### [ManagedBass](https://github.com/ManagedBass/ManagedBass)
Microphone Recording and Speaker Output (Wasapi Loopback) capture with mixing support.

`bass.dll` and `bassmix.dll` need to be placed in the same directory as `Captura.exe`.

<a href="https://nuget.org/packages/ManagedBass" id="MBassNuGet"></a>
[![Stars](https://img.shields.io/github/stars/ManagedBass/ManagedBass.svg?style=flat-square)](https://github.com/ManagedBass/ManagedBass/stargazers)
[![Forks](https://img.shields.io/github/forks/ManagedBass/ManagedBass.svg?style=flat-square)](https://github.com/ManagedBass/ManagedBass/fork)
[![Issues](https://img.shields.io/github/issues/ManagedBass/ManagedBass.svg?style=flat-square)](https://github.com/ManagedBass/ManagedBass/issues)

[![Gratipay](https://img.shields.io/gratipay/team/ManagedBass.svg?style=flat-square)](https://gratipay.com/ManagedBass/)
[![Build Status](https://img.shields.io/appveyor/ci/MathewSachin/ManagedBass.svg?style=flat-square)](https://ci.appveyor.com/project/MathewSachin/managedbass)
[![Chat](https://img.shields.io/gitter/room/ManagedBass/ManagedBass.svg?style=flat-square)](https://gitter.im/ManagedBass/ManagedBass)

### [NAudio](https://github.com/NAudio/NAudio)
Microphone Recording support when ManagedBass is not present.

<a href="https://nuget.org/packages/NAudio" id="NAudioNuGet"></a>
[![Stars](https://img.shields.io/github/stars/naudio/naudio.svg?style=flat-square)](https://github.com/naudio/naudio/stargazers)
[![Forks](https://img.shields.io/github/forks/naudio/naudio.svg?style=flat-square)](https://github.com/naudio/naudio/fork)
[![Issues](https://img.shields.io/github/issues/naudio/naudio.svg?style=flat-square)](https://github.com/naudio/naudio/issues)