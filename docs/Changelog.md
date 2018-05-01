# Changelog

## vNext

- Added Preview window as a video output.
- **fix:** `captura shot` failing for fullscreen screenshots.
- Separate colors for Right and Middle mouse click overlays.
- Support for custom image overlays.
- Stop Window capture when Window is closed.
- **fix:** Mouse cursor position is wrong after moving region selector.
- **fix:** Unable to resize Region Selector after stopping recording.
- `Duration` is considered after `Start Delay` has elapsed.
- `Duration` and `Start Delay` are stored in Settings.
- Added option to use System Proxy.
- **fix:** Mouse Cursor moves slowly when recording from Command-line
- Webcam capture support from Command-line.

## v7.0.1

- **fix:** FFMpeg download link gives 404.

## v7.0.0

- Improved efficiency of specific screen capture.
- The window is repositioned on startup if it is outside desktop bounds.
- Handle errors when copying to clipboard.
- Localization is done using JSON files.
- **fix:** Wrong full screen region size.
- Refresh updates the full screen region size.
- Desktop Duplication is now shown as a video source.
- Reuse `Image` and `Graphics` objects without disposing to reduce CPU usage.
- Better organisation in Settings file: `Captura.json`.
- Prevent crashes when possible.
- **fix:** Unable to precisely resize Region Selector on high DPI.
- Option to Hide Region Selector during recording.
- Lagarith codec support through SharpAvi (Install codec manually, disable Null Frames and select RGB as format).

### Overlays

- Added support for Custom Text overlays.
  `%elapsed%` can be used as a replacement token for the time elapsed.
- Opacity support for Webcam overlay.
- Remember selected webcam.
- Clicks & Keystrokes can be turned on/off during capture
- Hotkeys to Toggle Click and Keystrokes overlays (Alpha)

### FFMpeg

- Added Custom format for FFMpeg. Can be used by selecting encoder as `FFMpeg > Custom` and customizing arguments in `Config > FFMpeg`.
- Alpha: Live Streaming
- Alpha: NVENC support
- **fix:** Unable to record multiple screens with total width or height indivisible by 2.
- FFMpeg can be installed from the commandline.

## v6.0.1

- **fix:** High memory usage when recording a specific screen.

## v6.0.0

- Improved consistency and animations in the UI.
- **fix:** Prevent Variable Frame Rate GIF with Desktop Duplication.
- Improve audio-video synchronisation.
- **fix:** Screen recording with Desktop Duplication has an extra black region.
- **fix:** Error after recording without Desktop Duplication after trying to record with when it is not supported.
- **fix:** Error when Region Selector is taken out of visible bounds during recording.
- Added a simple drawing canvas.
- **fix:** Using Screenshot resize resulted in blank images.
- Pause recording on Sleep/Hibernate.

### Webcam

- **fix:** Webcam preview fails when Captura is run on dedicated graphics processor.
- Webcam drawn as overlay.
- Added `Configure | Webcam` for position and size of Webcam overlay.
- Opening Preview window is optional.
- Capture image button on header.

### MouseKeyHook

- MouseKeyHook customizations embedded in Config menu.
- Multipliers shown for repeated keystrokes.
- History of selected no of previous keystrokes displayed in overlay.
- **fix:** Corrected Keystrokes Center positioning.

### Installation

- Moving to standard type of installer where you the user has control over options.
- Chocolatey upgrade is now used again.

### Translation

- Language selection moved to Configure tab.
- Added lots of translations.

## v5.0.1

- Added Spanish Translation.
- Translations don't specify the country.

## v5.0.0

- Include Cursor cannot be changed during capture.
- Default hotkeys made simpler.
- Authenticated Proxy support.
- Settings saved in `APPDATA/Captura/Settings.json`. Recent Items and Hotkeys saved in separate files.
- **fix:** Variable Frame Rate Gif delay between frames.
- **fix:** Pausing with Variable Frame Rate Gif.
- **fix:** Region highlight borders not cleared after snapping.
- Shows error when unable to register hotkeys on start.
- Copy as Path option on Recent Items.
- Added support for using **Desktop Duplication API**.
- Removed Folder Writer due to high system resource consumption.
- Dropped frames are compensated with duplicated frames.
- MessageBox shown and Log file created on crash.
- Update Check

### UI

- Tabbed Configure page with Hotkeys and Extras embedded.
- Number Boxes to specify size of Region Selector.
- Added a Dark Theme.
- Removed Minimize to Tray checkbox.
- Removed selection of Topbar color.
- Added a Minimize to Tray button.

### Change in Audio/Video Sources

- **[Desktop]** under **Window** changed to **Full Screeb** under **Screen**.
- **Screen** kind is always visible.
- **No Video** renamed to **Only Audio**.
- **Audio alone Codecs** shown under **Only Audio** source.
- **WebCam** is selected on the Main Window.
- Window recording is of the initial size of window. Any resizing of the window is scaled to fit.

### Console

- **fix:** Video dimensions made even when capturing region from console.
- **fix:** Error running console with input redirected.
- **fix:** Use culture invariant string to represent region rectangle in console.

### ScreenShots

- ScreenShot Save Location and Format options on Main Tab.
- Support for uploading ScreenShots to **Imgur**.
- Option to hide MainWindow on **Full Screen** ScreenShot.

### FFMpeg

- FFMpeg is the default encoder.
- Audio and Video muxed in Realtime in FFMpeg capture.
- Added FFMpeg Downloader.
- Added FFMpeg HEVC Intel QSV encoder.
- FFMpeg is always shown in encoders list.
- A message with actions to take shown when trying to use FFMpeg in its absence.

## v4.2.2

**Fix:** Error notification after continuous recordings.

## v4.2.1

**Fix**: Crash when Snap to Window is clicked without dragging.
It now Snaps to the window below.

## v4.2.0

- **fix:** Fix thread exceptions when showing MessageBox.
- **fix:** NullReferenceException happening with Notifications.
- **fix:** High CPU usage in Recording and Paused states.
- Option to **Customize Clicks and Keystrokes overlay**.
- Better default style for the Clicks and Keystrokes overlay.
- Added Window capture Background Color.
- Added Opus support for FFMpeg audio alone capture.
- Improved Folder Browser Dialog (Contributed by [@armujahid](https://github.com/armujahid)).
- Added a circle around the Cursor, Clicks and Keys toggle options to improve accessibility.
- Gif Variable Frame Rate setting defaults to True.
- Added FFMpeg Logging. Openable from About tab.
- Resize/Rotate/Flip transforms now only for ScreenShots.

### Extras

A new window openable from About tab for additional settings:

- Accent Color
- Top Bar Color
- Setting Region Border Thickness
- Setting ScreenShot Notification Timeout
- Selecting Video Background Color
- Copying output file to Clipboard
- Making Main Window always to Top

## v4.1.0

- **fix:** Region Selector on High DPI.
- **fix:** Selected Video Writer Kind null if FFMpeg folder is removed when currently selected as Video Writer Kind.
- **fix:** Restores in normal state from `Minimize to Tray`.
- Added a Close button on Region Selector. When closed, Selected Video Source Kind changes to Window.
- Using MUI as a library.
- Output FileName support for start and shot commands.
- CommamdLine support for Transparent ScreenShots.
- New Encoder: Encode to Folder.

## v4.0.0

### Known Issues

- High memory usage when encoding Gif.
  **Workaround:** Use Variable Frame Rate.

### What's New

- Added [Command-line support](CmdLine.md).

## v3.5.0

### What's New

- **fix:** Crash when Recording stops automatically on reaching Duration.
- **fix:** Recording cannot start if Delay is set.
- **fix:** Optional libraries don't load on Chocolatey.
- Localization using RESX files. [Translations](Translation.md) are welcome.
- Scroll to change values in Number boxes.
- Removed NAudio support.
- Items in the Recent list are persisted only if the files exist.

## v3.4.0

### Known Issues

- Crash on recording stopped by Capture Duration.
- Recording cannot start if delay is set.
- Optional libraries don't load on Chocolatey.

### What's New

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

### Known Issues

- App may crash on Startup

### What's New

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

### Recent List

- Recent List persists items (max items can be configured).
- Added Recent list Clear button.
- **fix:** Scolling in Recent List.

### System Tray

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

### System Tray

- Video/Audio Recorded, ScreenShot Taken notifications.
- Notifications can be clicked to open corresponding file.
- Start/Stop/Pause/Resume Recording or Take ScreenShots or Exit from Tray.
- Added Minimize to System Tray option.

### Video/Audio Writers

- Video Witers grouped by Kind.
- Added FFmpeg encoding support (using ffmpeg.exe) (optional) (realtime when audio or video alone, post-processing when both).
- Dropped SharpAvi LAME Mp3 encoding support.
- Mp3 encoding supported using FFmpeg.

### Region Capture

- Region as Video Kind.
- Removed functionality of using RegionSelector in Window Video Kind.
- RegionSelector is more visible.
- Region Selector displays only when selected kind is Region.
- Region Selector can be moved but not resized during capture.
- Borders and Header of Region Selector are ignored in Region capture.

## Before v2.0.0

Chagelog not maintained for versions before v2.0.0.