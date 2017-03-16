---
layout: page
title: Frequently Asked Questions
permalink: FAQ/
reading_time: true
---

### Will Captura support Linux or Mac?
Captura is written using .NET Framework, which at present, is supported only on Windows.

Software written using .NET Framework can work on Linux and Mac using Mono but the native calls and UI pose a problem to that.

Also, the recently released .Net Core only has support for console applications.

### Does Captura support DirectX Game Video Recording?
Full Screen DirectX/OpenGL games CANNOT be recorded.
They run in a special mode where the current capture methods fail.

We have tried a good project: https://github.com/spazzarama/Direct3DHook but the performance needs to be tweaked before it can be used for Video Recording.

**There's a workaround:**  
If you can make the game run in windowed mode, you can capture it.
It's great if the game supports windowed mode or you can use a tool like [DxWnd](https://sourceforge.net/projects/dxwnd/).

### Why is Recorded Video of the size of entire screen when I capture a specific Window?
When capturing a specific Window, there is a possibility of that Window being moved or resized.
The video size must be big enough to encorporate resizing.
So, it is kept of the size of the entire screen.

If it's really necessary to keep the video of the size of the window, follow:

- Select `Region` as Video Source type instead of `Window`.
- Click on hold `Snap to Window` button on upper left of Region Selector.
- Drag and release on the Window to be captured.
- Start capture

### Why does the captured video playback very fast? ([#44](https://github.com/MathewSachin/Captura/issues/44))
This may happen if frames are not being captured as fast as the framerate set.
Try a lower value of framerate, 15fps would always work.
We admit that the technology employed in Captura is not that fast.

There are solutions like Fraps, Dxtory, OBS, and NVidia Share which provide 60fps recordings, but the technologies they use are complicated.

### Can I capture the User Account Control (UAC) screen? ([#31](https://github.com/MathewSachin/Captura/issues/31))
No. Not directly.

**There's a workaround for this:**
1. Run `gpedit.msc`
2. Navigate to `Computer Configuration\Windows Settings\Security Settings\Local Policies\SecurityOptions`.
3. Change **User Account Control: Switch to the secure desktop when prompting for elevation** to **Disabled**.

Undo this change after the screenshot, because it makes the system less secure!