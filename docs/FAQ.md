# Frequently Asked Questions

## Will Captura support Linux or Mac?
Captura is written using .NET Framework, which at present, is supported only on Windows.

Software written using .NET Framework can work on Linux and Mac using Mono but the native calls and UI pose a problem to that.

Also, the recently released .Net Core only has support for console applications.

## Does Captura support DirectX Game Video Recording?
Some games can be recorded when running on Windows 8 and above. In Captura v8.0.0 there was a separate `Desktop Duplication` option which can also record games which support that. From v9.0.0, `Desktop Duplication` is the default mode.

## Why is maximum frame rate 30fps?
Captura is not very fast on low-end systems. This limit on framerate is a protection against Captura consuming all of your CPU/Memory/Disk and causing your system to hang.

Starting from v8.0.0, you can remove this limitation by going to Config / Extras / Remove FPS Limit.

For reference, Captura can capture 1920x1080 screen at 40fps on my system without audio using FFmpeg x264 codec. My system specifications: 8GB RAM DDR4, Intel i5 6th Gen CPU 2.3 GHz, Windows 10.

## Why is the length of captured video shorter than recording duration?
This happens when Captura drops frames when your system can't keep with the specified frame rate. Try using a lower value of framerate, faster codec or smaller region.

## Why does Captura run out of resources (high memory/CPU/disk usage) during recording?
Atleast 2 GHz CPU and 4 GB RAM are recommended.

This may happen if frames are not being captured as fast as the framerate set. Try a lower value of framerate, faster codec or smaller region. Also, try terminating unnecessary applications running in background using Task Manager. We admit that the technology employed in Captura is not fast.

## Why does my Antivirus say that Captura is virus infected?
Captura is virus-free. It does not include any spam, adware or spyware.

It is probably due to the keystrokes capture feature being mistaken for a keylogger.