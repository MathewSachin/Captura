---
layout: page
title: System Tray - User Manual
reading_time: true
---

> THIS DOCUMENT IS CURRENTLY BEING WRITTEN.

Captura has a System tray icon made using [HardCodet.Wpf.TaskbarNotification](https://www.nuget.org/packages/Hardcodet.NotifyIcon.Wpf/) nuget package.

The icon changes depending on state: Not Recording, Recording, Paused.

Double clicking on the icon shows or hides the Main Window.

## Context Menu
Right clicking on the icon brings up the context menu with options:
- Start/Stop Recording
- Pause/Resume Recording
- ScreenShot
- ScreenShot Active Window
- ScreenShot Desktop
- Exit

## Notifications
Notifications are displayed when a recording or screenshot finishes saving.

ScreenShot notification displays a preview of the image.

Clicking on the red `X` dismisses the notifications.

Once you hover on a notification, it stays till manually dismissed.

Clicking on the notification opens the file that was saved.