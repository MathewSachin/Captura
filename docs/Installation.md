# Installation

There are a few different options for installing.

## Setup

Download and run `Captura-Setup.exe` for the latest release from [here][latest].

## Portable

Download and unzip `Captura-Portable.zip` for the latest release from [here][latest].

## Chocolatey

Things are even simpler if you use Chocolatey.
Just type `choco install captura -y` in your terminal.

## Dev Builds

[Dev Builds](CI.md) are also available.

The settings are stored in `%APPDATA%/Captura` folder by default. The settings folder can be customized using the `--settings` command-line argument.

## System Requirements

- Verified on **Windows 10**. Might work on earlier versions also.
- **.Net Framework v4.6.1** is required. You will be prompted to install if it is not already present on your system.
- Using the **FFMpeg Intel QSV HEVC** encoder requires the processor to be **Skylake (6th generation)** or later.
- **Desktop Duplication API** is only available on **Windows 8** and above.

## FFMpeg

[FFMpeg](http://ffmpeg.org/) is an open-source cross-platform solution to record, convert and stream audio and video.
It adds support for more output formats like **H.264** for Video and **Mp3**, **AAC** etc. when capturing **Only Audio**.

FFMpeg is configured on the **FFMpeg** section in the **Configure** tab.

Due to its large size (approx. 30MB), it is not included in the downloads.
If you already have FFMpeg on your system, you can just set the path to the folder containing it.
If it is installed globally (available in PATH), you don't have to do anything.
If you don't have FFMpeg or want to update, use the inbuilt **FFMpeg Downloader**.

[latest]: https://github.com/MathewSachin/Captura/releases/latest