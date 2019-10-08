# Command-line

Project      | Executable
-------------|----------------
UI           | captura.exe
Command-line | captura-cli.exe

We use the [CommandLineParser](https://nuget.org/packages/CommandLineParser) NuGet package.

The console projects uses default settings with a few modifications and does not save settings.

> Command-line support is not very stable. Please report any bugs you find.

### Why a separate Console app?
There are many issues related to using a WPF application as a console app.

- WPF applications don't block the console. So, we cannot use it for scenarios like wait till capture is completed.
- Writing to console does not work by default
- If `AttachConsole` is used, the written content interferes with console prompt.
- And more ...

### Command-line parameters for UI version

Argument     | Description
-------------|----------------------------------
--reset      | Reset all Settings
--tray       | Starts minimized to System Tray
--no-persist | Don't Save any changes in Settings
--no-hotkey  | Don't Register Hotkeys.
--settings   | Custom settings folder

e.g. Start captura minimized to tray

```
captura --tray
```

### Implemented Verbs

- [list](Verb-List.md)
  List available Screens, Windows, Audio Sources, Webcams, etc.
- [start](Verb-Start.md)
  Start a Recording
- [shot](Verb-Shot.md)
  Take a ScreenShot
- [ffmpeg](Verb-FFmpeg.md)  
  Allows installation of ffmpeg from command-line.
- help  
  Provides help on using the console app.
- version  
  Prints the version of the console app.