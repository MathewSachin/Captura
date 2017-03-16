---
layout: page
title: User Manual
reading_time: true
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
  In the paused state, the Pause icon remains rotated by 90 degrees.
5. **Timer:** Keeps record of the time-elapsed during Recording.
  It is reset when a new Recording is started.
6. **Minimize:** Minimizes the window.
7. **Close:** Exits Captura. Confirmation will be asked if a Recording is in progress.

## Tabs
![Tabs]({{ site.baseurl }}/img/Manual/2.png)

- [Main](main.html)
- Configure
- Recent
- [Hotkeys](hotkeys.html)
- About

## Bottom Section
![Bottom Section]({{ site.baseurl }}/img/Manual/3.png)

1. Displays the Selected folder where `ffmpeg.exe` is present. If this is empty, FFmpeg is used from the current folder or from the `PATH` environmental variable.
2. **Reset FFmpeg Folder:** Resets the FFmpeg folder to empty.
3. **Select FFmpeg Folder:** Shows a Folder Browser Dialog to select FFmpeg folder.
4. Displays the Selected output folder.
5. **Select Output Folder:** Shows a Folder Browser Dialog to select Output folder.
6. **Status Bar:** Displays status information.

## Other
- [Region Selector](region.html)
- [WebCam View](webcam.html)
- [System Tray](tray.html)
- [Command-line Support](cmdline.html)