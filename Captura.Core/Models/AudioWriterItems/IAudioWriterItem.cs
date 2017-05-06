﻿using Screna.Audio;

namespace Captura.Models
{
    public interface IAudioWriterItem
    {
        string Extension { get; }

        // file extension including the leading dot
        IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality);
    }
}
