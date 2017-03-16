---
layout: page
title: Feature Changelog
---

## March 2017
* Hotkeys for ScreenShot of Desktop and Active Window.
* Option to disable system tray notifications.
* NAudio fallback if BASS is not present.
* Implemented Audio Quality for FFMpeg.
* Fix errors in NumericBox.
* Remove Video BackgroundColor.
* Added Taskbar Thumb buttons.
* Keystrokes displayed on lower left.
* Embedded the Screna.* Extension libraries' code.
* Multiple output formats for Audio only recording.
* Implemented Video Quality for FFMpeg.

## Making Captura Better ([#48](https://github.com/MathewSachin/Captura/pull/48))
- Only _Screna.dll_ is a required dependency.
- Manually assignable Hotkeys for Starting/Stopping or Pausing/Resuming Recording and ScreenShot.
- IncludeCursor defaults to True.
- Recent List shows latest items first.
- Improved MouseKeyHook Overlay.
- Added Fallback when Transparent Window ScreenShot fails.
- Added Taskbar Overlay while Recording or Paused.
- Disable Configuration during Capture.

#### System Tray
- Video/Audio Recorded, ScreenShot Taken notifications.
- Notifications can be clicked to open corresponding file.
- Start/Stop/Pause/Resume Recording or Take ScreenShots or Exit from Tray.
- Added Minimize to System Tray option.

#### Video/Audio Writers
- Video Witers grouped by Kind.
- Added FFMpeg encoding support (using ffmpeg.exe) (optional) (realtime when audio or video alone, post-processing when both).
- Dropped SharpAvi LAME Mp3 encoding support.
- Mp3 encoding supported using FFMpeg.

#### Region Capture
- Region as Video Kind.
- Removed functionality of using RegionSelector in Window Video Kind.
- RegionSelector is more visible.
- Region Selector displays only when selected kind is Region.
- Region Selector can be moved but not resized during capture.
- Borders and Header of Region Selector are ignored in Region capture.

## January 2017
* Alpha support for Localisation.
* Recording both Microphone input and Speaker output.
