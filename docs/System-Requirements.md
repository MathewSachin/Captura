# System Requirements
System Requirements for the app to run.

## Operating System
Windows 10 is recommended.  
Atleast Windows 7 is required.  
If you are on Window 7, make sure *Aero* is enabled.

Screen recording is more efficient on Windows 8 and above as compared to Windows 7.

## Hardware
- 2 GHz CPU (Recommended)
- 4 GB RAM (Recommended)

## .NET Framework
[.NET Framework v4.7.2 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net472) is required.

## FFmpeg
The app automatically prompts to download FFmpeg if it is not already present on your system.

## Intel QSV
Using the **FFmpeg Intel QSV HEVC** encoder requires the processor to be **Skylake (6th generation)** or later.

## NVenc
See if your GPU supports NVenc for H.264 and H.265 in the [Support Matrix](https://developer.nvidia.com/video-encode-decode-gpu-support-matrix.)

## Lagarith codec
Lagarith codec can be used with SharpAvi.

- Install the codec from its [official website](https://lags.leetcode.net/codec.html).
- Make sure it is configured to use RGB mode.
- Make sure that Null Frames is disabled.

RGB mode and Null Frames can be configured from the registry:

##### HKEY_CURRENT_USER\Software\Lagarith:
- Mode = RGB
- NullFrames= 2294784