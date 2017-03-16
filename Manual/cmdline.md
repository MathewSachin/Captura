---
layout: page
title: Command-line Usage - User Manual
reading_time: true
---

Added a new Console app project: `Captura.Console` which generates `captura.exe`.  
The original WPF project generates `captura.ui.exe`.  

We use the [CommandLineParser](https://nuget.org/packages/CommandLineParser) NuGet package.

The console projects uses default settings with a few modifications and does not save settings.

## Why a separate Console app?
There are many issues related to using a WPF application as a console app.

- WPF applications don't block the console. So, we cannot use it for scenarios like wait till capture is completed.
- Writing to console does not work by default
- If `AttachConsole` is used, the written content interferes with console prompt.
- And more ...

## Implemented Verbs
- [list](#verb-list)
- [shot](#verb-shot)
- [start](#verb-start)

## Without a Verb
If a recognized verb is not used, `captura.exe` launches `captura.ui.exe` passing it the remaining arguments and exits.

### Valid arguments

Argument       | Description
---------------|-----------------------------------------
`--reset`      | Reset all Settings
`--tray`       | Starts minimized to System Tray
`--no-persist` | Don't Save any changes in Settings
`--no-hotkey`  | Don't Register Hotkeys.

e.g. Start captura minimized to tray

```
captura --tray
```

is same as:

```
captura.ui.exe --tray
```

## Verb: list
Displays the following information:

- Version
- If FFmpeg is available
- FFmpeg encoders
- If SharpAvi is available
- SharpAvi encoders
- If ManagedBass is available
- If MouseKeyHook is available
- Visible Windows with `hWnd`
- Screens if there are more than 1
- Available Microphones
- Available Speaker output sources

## Verb: shot
Takes a screenshot

### Valid arguments

Argument         | Description
-----------------|-------------------------------------
`--cursor`       | Include cursor in the screenshot
`--source`       | The source to take screenshot of. See [here](#using-the-source-argument).
`-f` or `--file` | Output file path.

e.g. Take a screenshot containing cursor.

```
captura shot --cursor
```

## Verb: start
Starts Recording.

There are two modes.
- When Length is specified, recording runs until specified Length.
- When Length is not specified, `press q to quit` message is displayed.

### Valid arguments

Argument              | Description
----------------------|---------------------------------------------
`--cursor`            | Include cursor
`--keys`              | Include keystrokes
`--clicks`            | Include mouse clicks
`--delay`             | Delay before starting recording (in ms)
`--length`            | Length of recording (in s)
`--source`            | The source to record from. See [here](#using-the-source-argument).
`--mic`               | The microphone index to use. (-1 = none (**Default**)) (0 is first device).
`--speaker`           | The speaker output index to use. (-1 = none (**Default**)) (0 is first device).
`-r` or `--framerate` | Frame Rate (**Default** is 10).
`--encoder`           | The video encoder to use. See [here](#using-the-encoder-argument).
`--vq`                | Video Quality (1 to 100) (**Default** is 70).
`--aq`                | Audio Quality (1 to 100) (**Default** is 50).
`-f` or `--file`      | Output file path.

e.g. Record 10 seconds with cursor and keystrokes and audio from first speaker output.

```
captura start --length 10 --cursor --keys --speaker=0
```

## Using the Source Argument
Using the source argument.

### Desktop
Use the `desktop` parameter to capture the entire Desktop (**Default**).
Works with both `captura start` and `captura shot`.
This is the default option, so is as good as not using this option.

e.g.

```
captura start --source desktop
```

### Region
Use Left, Top, Width and Height resp. as comma separated values to represent the region to capture.
Works with both `captura start` and `captura shot`.

e.g.

```
captura shot --source 100,100,300,400
```

### Screen
Use `screen:<index>` as the argument. `index` is a zero-based index identifying the screen.  
Works with both `captura start` and `captura shot`.  
You can use `captura list` to check screen indices.

e.g.

```
captura start --source screen:1
```

### No Video
Use `none` for No Video.  
Available only with `captura start`.  
Can be used for audio only recording.

### Window
Use `win:<hWnd>` as the argument. `hWnd` is handle of the window.  
Available only with `captura shot`.  
You can use `captura list` to check visible window handles.

## Using the Encoder argument

### Gif
```
captura start --encoder gif
```

### SharpAvi
Use `sharpavi:<index>` as argument. `index` is a zero-based index identifying the encoder.  
You can use `captura list` to check encoder indices.

### FFmpeg
Use `ffmpeg:<index>` as argument. `index` is a zero-based index identifying the encoder.  
You can use `captura list` to check encoder indices.