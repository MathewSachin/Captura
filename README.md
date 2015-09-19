# Captura
(c) 2015 Mathew Sachin. All Rights Reserved
Capture what's happering on your Screen along with audio from your microphone

Depends On
--------------------------------------------------------------
Fluent.dll (Downloadable from https://github.com/fluentribbon/Fluent.Ribbon/releases)
SharpAvi.dll (Buildable from https://github.com/MathewSachin/SharpAvi)
ManagedWin32.dll (Buildable from https://github.com/MathewSachin/ManagedWin32)
lameenc32.dll and lameenc64.dll (Contained in the Repository)

Features
--------------------------------------------------------------
Capture Screen
Capture Microphone Audio
Can Capture With/Without the Mouse Cursor
Ctrl+Shift+R: From Anywhere to Start/Stop Recording

MISSING FEATURE: LOOPBACK
--------------------------------------------------------------
Loopback requires Wasapi.
Tried using it through Bass.Net and NAudio.
Worked but the audio was lagging behind Video,
due to slowness of Wasapi over WaveIn in interaction with a .Net environment.

Help on the Same would be appreciated.

Acknowledgements
--------------------------------------------------------------
Uses a part of Mark Heath's NAudio for WaveIn,
SharpAvi.dll by Vasilli Masilov
and is influenced by the accompanying Sample App - Screencasts
and Fluent.dll Interface

Todo
--------------------------------------------------------------
1.  Save Output Folder in Settings
2.  ScreenCapture of DirectX fullscreen windows (using DWM)
3.  Wasapi Loopback Capture (NAudio) (including silence)
4.  Wpf folder browse dialog
5.  Capture Specific Window