---
layout: page
title: User Manual
permalink: User-Manual/
---

> THIS DOCUMENT IS CURRENTLY BEING WRITTEN.

The MainWindow cannot be manually resized.
It can be moved by holding the **Timer on Top bar** and dragging.

## Top Bar
![Top Bar]({{ site.baseurl }}/img/Manual/1.png)

1. **Expander:** Expand or collapse the interface.
  The expanded or collapsed state of the interface is persisted between reloads.
2. **ScreenShot:** Take a ScreenShot of the selected Video Source.
  Not Available when selected **Video Source Kind** is **No Video**.
3. **Record/Stop:** Start or Stop Recording.
  Not Available when selected **Video Source Kind** is **No Video** along with Audio Sources as **No Sound**.
4. **Pause/Resume:** Pause or Resume Recording.
  Only available when a recording is in progress.
  In the pause state, the Pause icon remains rotated by 90 degrees.
5. **Timer:** Keeps record of the time-elapsed during Recording.
  It is reset when a new Recording is started.
6. **Minimize:** Minimizes the window.
7. **Close:** Exits Captura. Confirmation will be asked if a Recording is in progress.

## Bottom Section
![Bottom Section]({{ site.baseurl }}/img/Manual/3.png)

1. Displays the Selected folder where `ffmpeg.exe` is present. If this is empty, FFmpeg is used from the current folder or from the `PATH` environmental variable.
2. **Reset FFmpeg Folder:** Resets the FFmpeg folder to empty.
3. **Select FFmpeg Folder:** Shows a Folder Browser Dialog to select FFmpeg folder.
4. Displays the Selected output folder.
5. **Select Output Folder:** Shows a Folder Browser Dialog to select Output folder.
6. **Status Bar:** Displays status information.

## Tabs
![Tabs]({{ site.baseurl }}/img/Manual/2.png)

- Main
- Configure
- Recent
- Hotkeys
- About

### Main Tab
![Main Tab]({{ site.baseurl }}/img/Manual/4.png)

**1. Include Cursor:** Display Mouse Cursor in Recordings and ScreenShots (Default is Yes). Can be toggled during Recording.

**2. Include Mouse Clicks:** Display Mouse Clicks in Recordings. Only available if MouseKeyHook addon is present.

**3. Include Keystrokes:** Display Keystrokes in Recordings. Keystrokes are displayed on the lower left of recording. Only available if MouseKeyHook addon is present.

**4. Refresh:** Refreshes the lists of Video Sources, Video Encoders and Audio Sources.

**5. Open Output Folder:** Opens the Output folder.

### Video

**6. Video Source**  
The first Combo-box contains types of Video sources.
The second Combo-box shows the available Video Sources of the selected type.
- **No Video:** You can use this for audio-alone recordings. ScreenShots are not available.
- **Window:** During Window capture, the recorded video is of the size of entire screen, but only the selected window is visible. This allows any resizing in the window to be captured.
  - **Desktop**
  - **Taskbar**
  - Other Visible Windows
- **Region:** Displays Region Selector.
- **Screen:** Only visible if more than one Screens are present.

**7. WebCam View:** Toggle Visibility of WebCam View.

**8. Video Encoder**  
Similar to Video Sources, the first Combo-box contains types of Video Encoders.
The second Combo-box shows the available Video Sources of the selected type.
- Gif
- FFmpeg
- SharpAvi

**9. Frame Rate:** No of frames captured per second (1 to 30) (Default is 10).

**0. Quality:** Quality of encoded video (1% - 100%) (Default is 70%).

### Audio
Audio from Microphone and Speaker output can be mixed together.

**A. Microphone Source:** Microphone input source.

**B. Speaker Output Source:** Speaker output source. Recorded using WASAPI Loopback.

**C. Audio format when no Video:** The format of encoded audio when there is no video (Video Source is **No Video**).
- Wave
- Mp3
- Aac
- Ogg

Audio encoders other than Wave use FFmpeg.

**D. Quality:** Quality of encoded audio (1% - 100%) (Default is 50%).