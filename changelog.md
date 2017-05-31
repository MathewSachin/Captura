---
layout: page
title: Feature Changelog
permalink: Changelog/
---

## v3.5.0
- **fix:** Crash when Recording stops automatically on reaching Duration.
- **fix:** Recording cannot start if Delay is set.
- **fix:** Optional libraries don't load on Chocolatey.
- Localization using RESX files. [Translations]({{ site.baseurl }}/Translation) are welcome.
- Scroll to change values in Number boxes.
- Removed NAudio support.
- Items in the Recent list are persisted only if the files exist.

## v3.4.0
- **fix:** App crashes on Startup in Release builds.
- Expanded/Collapsed state is persisted.
- Added MessageBoxes for showing some errors.
- **fix:** Cache Images in `ScreenShotBalloon`. Fixes the unability to delete files.
- Unconstrained Gif renamed to Variable Frame Rate.
- Confirm RecentItem deletion.
- **fix:** After recording tasks can run before saving is completed.
- Asks before Exit during Recording.
- **fix:** Hide Keystrokes and Clicks capture buttons when `MouseKeyHook` library is not present.

## v3.3.0
> Know Issue: App may crash on Startup

- **fix:** h264 (FFmpeg Mp4) encoder requires video dimensions to be even.
- **Webcam Support:** Not as a Video Source but as a standalone window which can be captured using standard methods.
- Redesigned RegionSelector.
- Implemented RegionSelector **Snap to Window**.
- **fix:** Prevent appearance of RegionSelector in Window list.
- **fix:** Prevent RegionSelector from being Maximized.
- **fix:** Heavy memory usage with FFmpeg.
- Collapsed/Expanded state of Main Window is persisted.

## v3.2.0
- .Net 4.6.1 is required.
- Remembers last used Audio/Video Codecs and Sources.
- Remembers Window Location.
- Remembers ScreenShot Save target and Image Format.
- Experimetal Gif encoding using FFmpeg.
- Rotate/Flip/Resize Transforms for ScreenShot/Video.
- **fix:** FFmpeg Command-line args when encoding both audio and video.
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
- Implemented Audio and Video Quality for FFmpeg.
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
- Added FFmpeg encoding support (using ffmpeg.exe) (optional) (realtime when audio or video alone, post-processing when both).
- Dropped SharpAvi LAME Mp3 encoding support.
- Mp3 encoding supported using FFmpeg.

#### Region Capture
- Region as Video Kind.
- Removed functionality of using RegionSelector in Window Video Kind.
- RegionSelector is more visible.
- Region Selector displays only when selected kind is Region.
- Region Selector can be moved but not resized during capture.
- Borders and Header of Region Selector are ignored in Region capture.

## Before v2.0.0
Chagelog not maintained for versions before v2.0.0.