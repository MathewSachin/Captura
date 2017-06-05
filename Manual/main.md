---
layout: page
title: Main Tab - User Manual
---

![Main Tab]({{ site.baseurl }}/img/Manual/main.png)

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
- **Region:** Displays [Region Selector](region.html).
- **Screen:** Only visible if more than one Screens are present.

**7. WebCam View:** Toggle Visibility of [WebCam View](webcam.html).

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