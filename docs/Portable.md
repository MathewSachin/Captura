# Portable

Starting from v9.0.0, for portable builds:
1. Settings are stored in `Settings` folder in app folder by default.
   This can be overridden by using the `--settings` command-line argument.
2. FFmpeg is downloaded into `Codecs` folder in app folder by default.
   This can be overriden from UI.

When setting FFmpeg folder, Settings folder or output folder, `%CAPTURA_PATH%` can be used to refer to the app folder.

e.g. `%CAPTURA_PATH%/Settings` means `Settings` folder in app folder.