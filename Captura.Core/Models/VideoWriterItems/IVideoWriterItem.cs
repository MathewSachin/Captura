﻿using Screna;
using Screna.Audio;

namespace Captura.Models
{
    /// <summary>
    /// Items to show in Video Writers list.
    /// </summary>
    public interface IVideoWriterItem
    {
        // file extension including the leading dot
        string Extension { get; }

        IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider, int AudioQuality, IAudioProvider AudioProvider);
    }
}
