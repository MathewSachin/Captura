---
layout: page
title: Feature Changelog
permalink: Changelog/
---

## v3.2.0
- .Net 4.6.1 is required.
- Remembers last used Audio/Video Codecs and Sources.
- Remembers Window Location.
- Remembers ScreenShot Save target and Image Format.
- Experimetal Gif encoding using FFMpeg.
- Rotate/Flip/Resize Transforms for ScreenShot/Video.
- **fix:** FFMpeg Command-line args when encoding both audio and video.
- **fix:** WASAPI Format error on loopback.

#### Recent List
- Recent List persists items (max items can be configured).
- Added Recent list Clear button.
- **fix:** Scolling in Recent List.

#### System Tray
- Icons in Context menu.
- Double click to show/hide MainWindow.
- Improved notifications.
- ScreenShot notification displays the image.
- Tray icon changes depending on the Recording state.

## v3.1.0
- `bass.dll` and `bassmix.dll` included in release.

## v3.0.0
- New Colors
- Added a button to reset hotkeys
- Hotkeys can be disabled.
- Expanded State on Launch.
- Hotkeys for ScreenShot of Desktop and Active Window.
- Option to disable system tray notifications.
- NAudio fallback if BASS is not present.
- Implemented Audio and Video Quality for FFMpeg.
- Fix errors in NumericBox.
- Remove Video BackgroundColor.
- Added Taskbar Thumb buttons.
- Keystrokes displayed on lower left.
- Embedded the Screna.* Extension libraries' code.
- Multiple output formats for Audio only recording.

## v2.0.0
- Only _Screna.dll_ is a required dependency.
- Manually assignable Hotkeys for Starting/Stopping or Pausing/Resuming Recording and ScreenShot.
- IncludeCursor defaults to True.
- Recent List shows latest items first.
- Improved MouseKeyHook Overlay.
- Added Fallback when Transparent Window ScreenShot fails.
- Added Taskbar Overlay while Recording or Paused.
- Disable Configuration during Capture.
- Alpha support for Localisation.
- Recording both Microphone input and Speaker output.

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

## Before v2.0.0
Chagelog not maintained for versions before v2.0.0.